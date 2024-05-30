using System.Runtime.Serialization;
namespace Protocols
{
    static public class InfExchange
    {
        public enum Option
        {
            Сonnecting,
            Chatting,
            Disconnecting
        }
        [Serializable]
        public class Message
        {
            Option option;
            byte[] data;
            int kol_bytes;
            public Option Mode { get { return option; } }
            public byte[] Data { get { return data; } }
            public int Bytes {  get { return kol_bytes; } }

            public Message(Option option, byte[] data)
            {
                this.option = option;
                this.data = (byte[])data.Clone();
                kol_bytes = data.Length;
            }
        }
    }
}
