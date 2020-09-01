using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public class Day23
    {
        private readonly List<Nic> network;

        public Day23(string program)
        {
            Nic bootNic(int id) => new Nic(program, id);
            network = Enumerable.Range(0, 50).Select(bootNic).ToList();
        }

        private class Nic : OpComputer
        {
            public Nic(string program, long id) : base(program) => Input(id);
        }

        public class Packet
        {
            public readonly int Address;
            public readonly long X;
            public readonly long Y;

            public Packet(int address, long x, long y)
            {
                Address = address;
                X = x;
                Y = y;
            }

            public List<long> toInputs() => new List<long> {X, Y};

            public override string ToString() => $"[{Address:00}] {X},{Y}";
        }

        public Packet GetFirstPacketSentTo(int targetAddress)
        {
            var i = 0;
            while (true)
            {
                var packet = runNic(network[i]);

                if (packet?.Address == targetAddress) return packet;
                sendPacket(packet);

                i = (i + 1) % 50;
            }
        }

        private void sendPacket(Packet packet)
        {
            if (packet == null) return;
            network[packet.Address].Input(packet.toInputs());
        }

        private Packet runNic(Nic nic)
        {
            var outputs = new List<long>();
            while (true)
            {
                switch (nic.Run())
                {
                    case (OpComputer.Response.Halt, _):
                        return null;

                    case (OpComputer.Response.Output, long value):
                        outputs.Add(value);
                        if (outputs.Count == 3)
                            return new Packet((int) outputs[0], outputs[1], outputs[2]);
                        break;

                    case (OpComputer.Response.AwaitingInput, _):
                        nic.Input(-1);
                        return null;
                }
            }
        }

        public long FirstDuplicateNatYValue()
        {
            var yValues = new HashSet<long>();
            var i = 0;
            var nullCount = 0;
            Packet lastNatPacket = null;

            while (true)
            {
                var packet = runNic(network[i]);

                if (packet == null)
                {
                    nullCount++;
                    if (nullCount > 500 && i == 49 && lastNatPacket != null)
                    {
                        if (yValues.Contains(lastNatPacket.Y))
                            return lastNatPacket.Y;
                        yValues.Add(lastNatPacket.Y);

                        network[0].Input(lastNatPacket.toInputs());
                        nullCount = 0;
                    }
                }

                else if (packet.Address == 255)
                    lastNatPacket = packet;

                else
                    sendPacket(packet);

                i = (i + 1) % 50;
            }

        }
    }
}
