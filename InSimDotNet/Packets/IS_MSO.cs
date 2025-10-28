using System;

namespace InSimDotNet.Packets {
    /// <summary>
    /// Message out packet.
    /// </summary>
    /// <remarks>
    /// Sent by LFS containing system and user messages.
    /// </remarks>
    public class IS_MSO : IPacket {
        private const int DefaultSize = 8;

        /// <summary>
        /// Gets the size of the packet.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Gets the type of the packet.
        /// </summary>
        public PacketType Type { get; private set; }

        /// <summary>
        /// Gets the packet request ID.
        /// </summary>
        public byte ReqI { get; private set; }

        /// <summary>
        /// Gets the unique ID of the connection who sent the message (0 if host).
        /// </summary>
        public byte UCID { get; private set; }

        /// <summary>
        /// Gets the unique ID of the player who sent the message (if 0 use UCID).
        /// </summary>
        public byte PLID { get; private set; }

        /// <summary>
        /// Gets the source that the message was sent from.
        /// </summary>
        public UserType UserType { get; private set; }

        /// <summary>
        /// Gets the first character of the actual text (after player name).
        /// </summary>
        public byte TextStart { get; private set; }

        /// <summary>
        /// Gets the full message (name + text).
        /// </summary>
        public string Msg { get; private set; }

        /// <summary>
        /// Gets the first character of the actual raw text (after player name).
        /// </summary>
        public byte RawTextStart { get; private set; }

        /// <summary>
        /// Gets the raw bytes of the <see cref="Msg"/> string.
        /// </summary>
        public byte[] RawMsg => _rawMsg;
        private readonly byte[] _rawMsg;

        /// <summary>
        /// Gets the text message (it doesn't include player name).
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Creates a new message out packet.
        /// </summary>
        public IS_MSO() {
            Size = DefaultSize;
            Type = PacketType.ISP_MSO;
            Msg = string.Empty;
        }

        /// <summary>
        /// Creates a new message out packet.
        /// </summary>
        /// <param name="buffer">A buffer contaning the packet data.</param>
        public IS_MSO(byte[] buffer)
            : this() {
            PacketReader reader = new PacketReader(buffer);
            Size = reader.ReadSize();
            Type = (PacketType)reader.ReadByte();
            ReqI = reader.ReadByte();
            reader.Skip(1);
            UCID = reader.ReadByte();
            PLID = reader.ReadByte();
            UserType = (UserType)reader.ReadByte();
            RawTextStart = reader.ReadByte();

            // the length of the message will be the size of the packet minus all the above crap.
            int msgLength = Size - DefaultSize;
            _rawMsg = reader.ReadBytes(msgLength);

            // Need to correct textstart after we've converted string to unicode.
            if (RawTextStart > 0) {
                string preText = LfsEncoding.Current.GetString(RawMsg, 0, RawTextStart);
                Text = LfsEncoding.Current.GetString(RawMsg, RawTextStart, RawMsg.Length - RawTextStart);
                TextStart = (byte)preText.Length;
                Msg = preText + Text;
            }
            else {
                Text = LfsEncoding.Current.GetString(RawMsg, 0, RawMsg.Length);
                TextStart = 0;
                Msg = Text;
            }
        }
    }
}
