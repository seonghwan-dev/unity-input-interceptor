namespace Calci
{
    public interface IKeyCodeHandler
    {
        int Handle(ushort VirtualKey);
    }

    public class DefaultKeyCodeHandler : IKeyCodeHandler
    {
        public virtual int Handle(ushort VirtualKey)
        {
            // alphabets
            if (VirtualKey >= 0x41 && VirtualKey <= 0x5A)
            {
                return (int)(32 + VirtualKey);
            }
            
            // numbers
            if (VirtualKey >= 0x30 && VirtualKey <= 0x39)
            {
                return VirtualKey;
            }
            
            // number pads
            if (VirtualKey >= 0x60 && VirtualKey <= 0x69)
            {
                return VirtualKey + (0x100 - 0x60);
            }
            
            // functions
            if (VirtualKey >= 0x70 && VirtualKey <= 0x7B)
            {
                return VirtualKey + (0x11A - 0x70);
            }

            return 0;
        }
    }
}