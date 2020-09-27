using System.IO;

namespace MightyStruct.Core
{
    public interface INode
    {
        INode Root { get; }
        INode Parent { get; }

        Stream Stream { get; }
    }

    public class Node<T> : INode
    {
        public INode Root { get; }
        public INode Parent { get; }

        public Stream Stream { get; }

        public ISerializer<T> Serializer { get; }

        private bool _readValue;
        private T _value;

        public T Value
        {
            get
            {
                if (_readValue) return _value;

                Stream.Seek(0, SeekOrigin.Begin);

                _value = Serializer.ReadFromStream(Stream);
                _readValue = true;

                return _value;
            }
            set
            {
                _value = value;

                Stream.Seek(0, SeekOrigin.Begin);
                Serializer.WriteToStream(Stream, _value);
            }
        }

        public Node(INode parent, Stream stream, ISerializer<T> serializer)
        {
            Parent = parent;
            Root = Parent.Root;

            Stream = stream;
            Serializer = serializer;
        }
    }
}
