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
                        case 5:
                        case 14:
                        case 17:
                            this.Add(new AbsoluteMovement(type, reader.ReadBytes(17)));
                            break;

                        case 1:
                        case 2:
                        //case 12:
                        //case 13:
                            case 16:
                            this.Add(new RelativeMovement(type, reader.ReadBytes(7)));
                            break;

                        case 3:
                        case 4:
                        case 6:
                        case 8:
                        case 11:
                        case 15:
                            this.Add(new InstantMovement(type, reader.ReadBytes(9)));
                            break;

                        case 9:
                            this.Add(new EquipmentMovement(type, reader.ReadBytes(1)));
                            break;/*

					case 10:
						this.Add(new ChairMovement(type, reader.ReadBytes(9)));
						break;*/

                        case 12:
                        case 24:
                            this.Add(new JumpDownMovement(type, reader.ReadBytes(19)));
                            break;

                        case 20: //TODO: FJ
                            break;

                        case 19:
                        case 21:
                        case 22:
                        case 26: //TODO: Aran
                            reader.Skip(7);
                            break;

                        case 27: //TODO: Knockback
                            reader.Skip(9);
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
        public short FootHold { get; set; }
        public int Unknown { get; set; }
        public short Duration { get; set; }

        public AbsoluteMovement(byte type, byte[] data)
            : base(type)
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                this.Position = new Point(reader.ReadShort(), reader.ReadShort());
                this.Wobble = new Point(reader.ReadShort(), reader.ReadShort());
                this.FootHold = reader.ReadShort();
                this.Unknown = reader.ReadInt();
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
                writer.WriteShort(this.FootHold);
                writer.WriteInt(this.Unknown);
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
        public int Unknown { get; set; }
        public short Duration { get; set; }
        public int FootHold { get; set; }

        public JumpDownMovement(byte type, byte[] data)
            : base(type)
        {
            using (ByteBuffer reader = new ByteBuffer(data))
            {
                this.Position = new Point(reader.ReadShort(), reader.ReadShort());
                this.Wobble = new Point(reader.ReadShort(), reader.ReadShort());
                this.FootHold = reader.ReadInt();
                this.Unknown = reader.ReadInt();
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
                writer.WriteInt(this.Unknown);
                writer.WriteInt(this.FootHold);
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
