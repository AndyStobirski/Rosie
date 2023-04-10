namespace Rosie.Code.Interfaces
{
    public interface iOpenable
    {
        public bool Open();
        public bool Close();

        public bool IsOpen { get; set; }
    }
}
