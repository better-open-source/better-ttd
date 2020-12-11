using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BetterTTD.Domain.Enums;

namespace BetterTTD.Coan_OLD.Networks
{
    public class RconBuffer : IEnumerable<RconBuffer.Entry>
    {
        public record Entry(string Message, Colors Color);

        private readonly List<Entry> _buffer = new();
        private bool _eor;
        
        public IEnumerator<Entry> GetEnumerator()
        {
            return _buffer.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _buffer.GetEnumerator();
        }

        public void Add(string message, Colors color)
        {
            _buffer.Add(new Entry(message, color));
        }

        public void SetEOR()
        {
            _eor = true;
        }

        public bool IsEOR()
        {
            return _eor;
        }
        
        public int Length()
        {
            return _buffer.Count;
        }

        public bool IsEmpty()
        {
            return _buffer.Any();
        }
    }
}