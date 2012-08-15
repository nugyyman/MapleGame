using System.Collections.Generic;
using Loki.IO;

namespace Loki.Maple
{
    public class Movements : List<Movement>
    {
        public static Movements Parse(byte[] data)
        {
            return new Movements(data);
        }

        public Movements(byte[] data)
            : base()
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                byte commands = reader.ReadByte();

                for (byte b = 0; b < commands; b++)
                {
                    byte type = reader.ReadByte();

                    switch (type)
                    {
                        case 0:
                        case 7:
                        case 16:
                        case 45:
                        case 46:
                        case 60:
                        case 61:
                        case 62:
                            this.Add(new AbsoluteMovement(type, reader.ReadBytes(17)));
                            break;

                        case 1:
                        case 2:
                        case 15:
                        case 18:
                        case 21:
                        case 40:
                        case 41:
                        case 42:
                        case 43:
                            this.Add(new RelativeMovement(type, reader.ReadBytes(7)));
                            break;

                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 8:
                        case 9:
                        case 10:
                        case 12:
                        case 13:
                            this.Add(new InstantMovement(type, reader.ReadBytes(9)));
                            break;

                        case 11:
                            this.Add(new EquipmentMovement(type, reader.ReadBytes(1)));
                            break;

                        case 14:
                            this.Add(new JumpDownMovement(type, reader.ReadBytes(19)));
                            break;

                        case 20: //TODO: FJ
                            break;

                        case 17:
                        case 22:
                        case 23:
                        case 24:
                        case 25: //?
                        case 26: //?
                        case 27: //?
                        case 28: //?
                        case 29: //? <- has no offsets
                        case 30:
                        case 31:
                        case 32:
                        case 35: // i think, well in gms anyway
                        case 36: //this too
                        case 37:
                        case 38:
                        case 33:
                        case 34:
                        case 39://TODO: Aran
                            reader.Skip(7);
                            break;
                    }
                }
            }
        }

        public byte[] ToByteArray()
        {
            using (ByteBuffer writer = new ByteBuffer())
            {
                writer.WriteByte((byte)this.Count);

                foreach (Movement movement in this)
                {
                    writer.WriteBytes(movement.ToByteArray());
                }

                writer.Flip();

                return writer.GetContent();
            }
        }
    }

    public abstract class Movement
    {
        public byte Type { get; private set; }
        public byte NewStance { get; set; }

        public Movement(byte type)
        {
            this.Type = type;
        }

        public abstract byte[] ToByteArray();
    }

    public class AbsoluteMovement : Movement
    {
        public Point Position { get; set; }
        public Point Wobble { get; set; }
        public short Unknown { get; set; }
        public Point Offset { get; set; }
        public short Duration { get; set; }

        public AbsoluteMovement(byte type, byte[] data)
            : base(type)
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                this.Position = new Point(reader.ReadShort(), reader.ReadShort());
                this.Wobble = new Point(reader.ReadShort(), reader.ReadShort());
                this.Unknown = reader.ReadShort();
                this.Offset = new Point(reader.ReadShort(), reader.ReadShort());
                this.NewStance = reader.ReadByte();
                this.Duration = reader.ReadShort();
            }
        }

        public override byte[] ToByteArray()
        {
            using (ByteBuffer writer = new ByteBuffer())
            {
                writer.WriteByte(this.Type);
                writer.WriteShort(this.Position.X);
                writer.WriteShort(this.Position.Y);
                writer.WriteShort(this.Wobble.X);
                writer.WriteShort(this.Wobble.Y);
                writer.WriteShort(this.Unknown);
                writer.WriteShort(this.Offset.X);
                writer.WriteShort(this.Offset.Y);
                writer.WriteByte(this.NewStance);
                writer.WriteShort(this.Duration);

                writer.Flip();
                return writer.GetContent();
            }
        }
    }

    public class JumpDownMovement : Movement
    {
        public Point Position { get; set; }
        public Point Wobble { get; set; }
        public short Unknown { get; set; }
        public short Duration { get; set; }
        public short FootHold { get; set; }
        public Point Offset { get; set; }

        public JumpDownMovement(byte type, byte[] data)
            : base(type)
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                this.Position = new Point(reader.ReadShort(), reader.ReadShort());
                this.Wobble = new Point(reader.ReadShort(), reader.ReadShort());
                this.Unknown = reader.ReadShort();
                this.FootHold = reader.ReadShort();
                this.Offset = new Point(reader.ReadShort(), reader.ReadShort());
                this.NewStance = reader.ReadByte();
                this.Duration = reader.ReadShort();
            }
        }

        public override byte[] ToByteArray()
        {
            using (ByteBuffer writer = new ByteBuffer())
            {
                writer.WriteByte(this.Type);
                writer.WriteShort(this.Position.X);
                writer.WriteShort(this.Position.Y);
                writer.WriteShort(this.Wobble.X);
                writer.WriteShort(this.Wobble.Y);
                writer.WriteShort(this.Unknown);
                writer.WriteShort(this.FootHold);
                writer.WriteShort(this.Offset.X);
                writer.WriteShort(this.Offset.Y);
                writer.WriteByte(this.NewStance);
                writer.WriteShort(this.Duration);

                writer.Flip();
                return writer.GetContent();
            }
        }
    }

    public class RelativeMovement : Movement
    {
        public Point Delta { get; set; }
        public short Duration { get; set; }

        public RelativeMovement(byte type, byte[] data)
            : base(type)
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                this.Delta = new Point(reader.ReadShort(), reader.ReadShort());
                this.NewStance = reader.ReadByte();
                this.Duration = reader.ReadShort();
            }
        }

        public override byte[] ToByteArray()
        {
            using (ByteBuffer writer = new ByteBuffer())
            {
                writer.WriteByte(this.Type);
                writer.WriteShort(this.Delta.X);
                writer.WriteShort(this.Delta.Y);
                writer.WriteByte(this.NewStance);
                writer.WriteShort(this.Duration);

                writer.Flip();
                return writer.GetContent();
            }
        }
    }

    public class InstantMovement : Movement
    {
        public Point Position { get; set; }
        public Point Wobble { get; set; }

        public InstantMovement(byte type, byte[] data)
            : base(type)
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                this.Position = new Point(reader.ReadShort(), reader.ReadShort());
                this.Wobble = new Point(reader.ReadShort(), reader.ReadShort());
                this.NewStance = reader.ReadByte();
            }
        }

        public override byte[] ToByteArray()
        {
            using (ByteBuffer writer = new ByteBuffer())
            {
                writer.WriteShort(this.Position.X);
                writer.WriteShort(this.Position.Y);
                writer.WriteShort(this.Wobble.X);
                writer.WriteShort(this.Wobble.Y);
                writer.WriteByte(this.NewStance);

                writer.Flip();
                return writer.GetContent();
            }
        }
    }

    public class EquipmentMovement : Movement
    {
        public byte Unknown { get; set; }

        public EquipmentMovement(byte type, byte[] data)
            : base(type)
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                this.Unknown = reader.ReadByte();
            }
        }

        public override byte[] ToByteArray()
        {
            using (ByteBuffer writer = new ByteBuffer())
            {
                writer.WriteByte(this.Type);
                writer.WriteByte(this.Unknown);

                writer.Flip();
                return writer.GetContent();
            }
        }
    }
}
