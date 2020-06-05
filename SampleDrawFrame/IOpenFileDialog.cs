namespace SampleDrawFrame
{
    public interface IOpenFileDialog
    {
        bool? ShowDialog();

        string FileName { get; }
    }
}
