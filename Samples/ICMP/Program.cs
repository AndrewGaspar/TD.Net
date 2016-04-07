using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TD;
using static TD.Standard;

namespace ICMP
{
    [Flags]
    internal enum IPFlags
    {
        MoreFragments = 0x1,
        DontFragment = 0x2,
        Reserved = 0x4,
    }

    internal static class Util
    {
        public static ITransducer<IEnumerable<byte>, T> NetworkBytesToNumeric<T>() where T : struct =>
            ConvertToNumeric<T>().HostOrder();

        public static T GetNetworkNumeric<T>(IEnumerable<byte> bytes) where T : struct =>
            ForOne.ValueTransduce(
                bytes,
                NetworkBytesToNumeric<T>()).Value;

        public static ushort CalculateChecksum(IEnumerable<byte> bytes)
        {
            var sum =
                bytes.Reduce<byte, uint>(
                        0,
                        Framing<byte>(2)
                            .Mapping(frame => (uint)(frame[0] << 8) + frame[1])
                            .Apply(Accumulator.Unchecked<uint>())).Value;

            Func<uint, byte> getCarry = value => (byte)((value & 0xF0000) >> 16);

            var carry = getCarry(sum);

            while (carry > 0)
            {
                sum &= 0xFFFF;
                sum += carry;

                carry = getCarry(sum);
            }

            var checksum = (ushort)~sum;
            var upperByte = (ushort)((checksum & 0xFF00) >> 8);
            var lowerByte = (ushort)((checksum & 0xFF) << 8);
            return (ushort)(upperByte + lowerByte);
        }
    }

    internal class IP
    {
        public byte[] Packet { get; private set; }

        protected static byte[] GetNetworkOrderBytes<T>(T value) where T : struct =>
            ForOne.Transduce(value, Net.NetworkOrder<T>().ConvertToBytes());

        protected static T ReadValueFromPacket<T>(IList<byte> value, int index) where T : struct =>
            Util.GetNetworkNumeric<T>(value.Skip(index).Take(Marshal.SizeOf<T>()));

        private void SetByte(int index, int mask, int shift, byte value)
        {
            // clear the destination bits
            Packet[index] &= (byte)~mask;
            Packet[index] |= (byte)(value << shift);
        }

        private void SetField(int index, int mask, int shift, int size, byte value)
        {
            var max = (byte)((1 << size) - 1);
            if (value > max) throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be greater than {max}.");

            SetByte(index, mask, shift, value);
        }

        private byte GetField(int index, byte mask, int shift) =>
            (byte)((Packet[index] & mask) >> shift);

        public byte IPVersion
        {
            get
            {
                return GetField(index: 0, mask: 0xF0, shift: 4);
            }
            set
            {
                SetField(index: 0, mask: 0xF0, shift: 4, size: 4, value: value);
            }
        }

        public byte IHL
        {
            get
            {
                return GetField(index: 0, mask: 0xF, shift: 0);
            }
            set
            {
                SetField(index: 0, mask: 0xF, shift: 0, size: 4, value: value);
            }
        }

        public int IPHeaderLength => IHL * 4;

        public byte DSCP
        {
            get
            {
                return GetField(index: 1, mask: 0xFC, shift: 2);
            }
            set
            {
                SetField(index: 1, mask: 0xFC, shift: 2, size: 6, value: value);
            }
        }

        public byte ECN
        {
            get
            {
                return GetField(index: 1, mask: 0x3, shift: 0);
            }
            set
            {
                SetField(index: 1, mask: 0x3, shift: 0, size: 2, value: value);
            }
        }

        public ushort TotalLength
        {
            get
            {
                return ReadValueFromPacket<ushort>(Packet, 2);
            }
            set
            {
                GetNetworkOrderBytes(value).CopyTo(Packet, 2);
            }
        }

        public ushort Identification
        {
            get
            {
                return ReadValueFromPacket<ushort>(Packet, 4);
            }
            set
            {
                GetNetworkOrderBytes(value).CopyTo(Packet, 4);
            }
        }

        public IPFlags Flags
        {
            get
            {
                return (IPFlags)GetField(index: 6, mask: 0xC0, shift: 5);
            }
            set
            {
                SetField(index: 6, mask: 0xC0, shift: 5, size: 3, value: (byte)value);
            }
        }

        public ushort FragmentOffset
        {
            get
            {
                return (ushort)(ReadValueFromPacket<ushort>(Packet, 6) & 0x1FFF);
            }
            set
            {
                if (value >= 0x2000) throw new ArgumentOutOfRangeException(nameof(value), "FragmentOffset must be less than 0x2000");

                var bytes = GetNetworkOrderBytes(value);
                SetByte(index: 6, mask: 0x1F, shift: 0, value: bytes[0]);
                Packet[7] = bytes[1];
            }
        }

        public byte TimeToLive
        {
            get
            {
                return Packet[8];
            }
            set
            {
                Packet[8] = value;
            }
        }

        public byte IPProtocol
        {
            get
            {
                return Packet[9];
            }
            set
            {
                Packet[9] = value;
            }
        }

        public ushort IPChecksum
        {
            get
            {
                return ReadValueFromPacket<ushort>(Packet, 10);
            }
            set
            {
                GetNetworkOrderBytes(value).CopyTo(Packet, 10);
            }
        }

        public IPAddress SourceAddress
        {
            get
            {
                return new IPAddress(Packet.Skip(12).Take(4).ToArray());
            }
            set
            {
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    throw new ArgumentException(nameof(value), "Only IPv4 addresses allowed.");

                value.GetAddressBytes().CopyTo(Packet, 12);
            }
        }

        public IPAddress DestinationAddress
        {
            get
            {
                return new IPAddress(Packet.Skip(16).Take(4).ToArray());
            }
            set
            {
                if (value.AddressFamily != AddressFamily.InterNetwork)
                    throw new ArgumentException(nameof(value), "Only IPv4 addresses allowed.");

                value.GetAddressBytes().CopyTo(Packet, 16);
            }
        }

        public Slice<byte, byte[]> IPHeader => new Slice<byte, byte[]>(Packet, 0, IPHeaderLength);
        public Slice<byte, byte[]> IPPayload => new Slice<byte, byte[]>(Packet, IPHeaderLength);

        public IP(byte[] packet)
        {
            Packet = packet;
        }

        public IP(int payloadSize)
        {
            Packet = new byte[20 + payloadSize];

            IPVersion = 4;
            IHL = 5;
            TotalLength = (ushort)Packet.Length;
        }

        public virtual void UpdateBookkeeping()
        {
            IPChecksum = 0;
            IPChecksum = Util.CalculateChecksum(IPHeader);
        }
    }

    internal enum ICMPMessageType
    {
        EchoReply = 0,
        DestinationUnreachable = 3,
        SourceQuench = 4,
        RedirectMessage = 5,
        EchoRequest = 8,
        RouterAdvertisement = 9,
        RouterSolicitation = 10,
        TimeExceeded = 11,
        ParameterProblemBadIPHeader = 12,
        Timestamp = 13,
        TimestampReply = 14,
        InformationRequest = 15,
        InformationReply = 16,
        AddressMaskRequest = 17,
        AddressMaskReply = 18,
        Traceroute = 30
    }

    internal class ICMP : IP
    {
        public Slice<byte, byte[]> ICMPPacket => new Slice<byte, byte[]>(Packet, IPHeaderLength);

        public ICMP(int payloadSize) : base(payloadSize + 8)
        {
            IPProtocol = 1;
        }

        public ICMP(byte[] packet) : base(packet) { }

        public ICMPMessageType ICMPType
        {
            get
            {
                return (ICMPMessageType)ICMPPacket[0];
            }
            set
            {
                ICMPPacket[0] = (byte)value;
            }
        }

        public byte ICMPSubtype
        {
            get
            {
                return ICMPPacket[1];
            }
            set
            {
                ICMPPacket[1] = value;
            }
        }

        public ushort ICMPChecksum
        {
            get
            {
                return ReadValueFromPacket<ushort>(ICMPPacket, 2);
            }
            set
            {
                GetNetworkOrderBytes(value).CopyTo(ICMPPacket.List, ICMPPacket.StartIndex + 2);
            }
        }

        public Slice<byte, byte[]> ICMPData
        {
            get
            {
                return new Slice<byte, byte[]>(ICMPPacket, 4, 4);
            }
        }

        public Slice<byte, byte[]> ICMPHeader
        {
            get
            {
                return new Slice<byte, byte[]>(ICMPPacket, 0, 8);
            }
        }

        public Slice<byte, byte[]> ICMPPayload
        {
            get
            {
                return new Slice<byte, byte[]>(ICMPPacket, 8);
            }
        }

        public override void UpdateBookkeeping()
        {
            ICMPChecksum = 0;

            base.UpdateBookkeeping();

            ICMPChecksum = Util.CalculateChecksum(IPPayload);
        }
    }

    internal class ICMPEchoPacket : ICMP
    {
        protected ICMPEchoPacket(int payloadSize) : base(payloadSize)
        {
        }

        public ICMPEchoPacket(byte[] packet) : base(packet) { }

        public ushort ICMPEchoIdentifier
        {
            get
            {
                return Util.GetNetworkNumeric<ushort>(ICMPData.Take(2));
            }
            set
            {
                GetNetworkOrderBytes(value).CopyTo(Packet, IPHeaderLength + 4);
            }
        }

        public ushort ICMPEchoSequenceNumber
        {
            get
            {
                return Util.GetNetworkNumeric<ushort>(ICMPData.Skip(2).Take(2));
            }
            set
            {
                GetNetworkOrderBytes(value).CopyTo(Packet, IPHeaderLength + 6);
            }
        }
    }

    internal class ICMPEchoRequestPacket : ICMPEchoPacket
    {
        public ICMPEchoRequestPacket(int payloadSize) : base(payloadSize)
        {
            ICMPType = ICMPMessageType.EchoRequest;
        }

        public ICMPEchoRequestPacket(byte[] packet) : base(packet) { }
    }
    
    class Program
    {
        private static ITransducer<byte, string> PacketFormatting =
            Framing<byte>(4)
                .Mapping(frame =>
                    string.Format("{0:X2}{1:X2}{2:X2}{3:X2}",
                        frame[0], frame[1], frame[2], frame[3]));

        static void Main(string[] args)
        {
            var bytes = new byte[] {
                0x45, 0x00, 0x00, 0x1C,
                0x00, 0x00, 0x40, 0x00,
                0x40, 0x01, 0xA5, 0x23,
                0xC0, 0xA8, 0x0A, 0x01,
                0xC0, 0xA8, 0x0A, 0x6C,
                0x08, 0x00, 0x13, 0x8B,
                0xE4, 0x74, 0x00, 0x00,
            };

            var samplePacket = new ICMPEchoRequestPacket(bytes);
            var originalIPChecksum = samplePacket.IPChecksum;
            var originalICMPChecksum = samplePacket.ICMPChecksum;

            samplePacket.UpdateBookkeeping();

            Console.WriteLine($"IP: Original {originalIPChecksum:X4}, Updated: {samplePacket.IPChecksum:X4}");
            Console.WriteLine($"ICMP: Original {originalICMPChecksum:X4}, Updated: {samplePacket.ICMPChecksum:X4}");

            //if(Util.CalculateChecksum(bytes) != 0x4d56)
            //{
            //    Console.WriteLine("Nooo!");
            //    Console.ReadKey();
            //    return;
            //}
            //else
            //{
            //    Console.WriteLine("We good");
            //}


            var icmpPacket = new ICMPEchoRequestPacket(0x20)
            {
                ICMPEchoIdentifier = 1,
                ICMPEchoSequenceNumber = 8,
                TimeToLive = 55,
                Flags = IPFlags.DontFragment,
                SourceAddress = IPAddress.Parse("192.168.10.108"),
                DestinationAddress = IPAddress.Parse("192.168.10.1"),
            };

            Enumerable.Range(0x61, 0x17)
                      .Select(x => (byte)x)
                      .Concat(Enumerable.Range(0x61, 0x9).Select(x => (byte)x))
                      .ToArray().CopyTo(icmpPacket.ICMPPayload.List, icmpPacket.ICMPPayload.StartIndex);

            icmpPacket.UpdateBookkeeping();

            icmpPacket.Packet.Reduce(
                Console.Out,
                PacketFormatting.Apply(TextIO.WriteLineReducer<string>()));

            var requestThread = new Thread(() =>
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp))
                {
                    socket.Connect(icmpPacket.DestinationAddress, 0);

                    var payload = icmpPacket.ICMPPacket.ToArray();
                    
                    for (var i = 0; i < 10; i++)
                    {
                        socket.Send(payload);
                        Thread.Sleep(1000);
                    }
                }
            });

            var responseThread = new Thread(() =>
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp))
                {
                    socket.Bind(new IPEndPoint(icmpPacket.SourceAddress, 0));
                    socket.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0 });

                    var i = 0;
                    while (true)
                    {
                        var result = new byte[100];
                        var numBytes = socket.Receive(result);

                        var packet = new ICMPEchoPacket(result.Take(numBytes).ToArray());

                        //if (packet.SourceAddress.Equals(icmpPacket.SourceAddress)) continue;

                        Console.WriteLine("Packet {0}", i++);

                        result.Take(numBytes).Reduce(
                            Console.Out,
                            PacketFormatting.Apply(TextIO.WriteLineReducer<string>()));

                        Console.WriteLine();
                    }
                }
            });

            responseThread.Start();
            requestThread.Start();

            while(true) { Thread.Sleep(10); }
        }
    }
}
