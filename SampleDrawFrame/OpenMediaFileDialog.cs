using Microsoft.Win32;

namespace SampleDrawFrame
{
    public class OpenMediaFileDialog : IOpenFileDialog
    {
        private readonly OpenFileDialog openFileDialog;

        public string FileName { get; private set; }

        public OpenMediaFileDialog()
        {
            openFileDialog = new OpenFileDialog()
            {
                Filter = "Видео (*.MP4)|*.MP4|Изображения (*.JPG,*.JPEG,*.PNG)|*.JPG;*.JPEG;*.PNG",
                Multiselect = false
            };
        }

        public bool? ShowDialog()
        {
            var result = openFileDialog.ShowDialog();

            if (result == true)
                FileName = openFileDialog.FileName;

            return result;
        }
    }
}
