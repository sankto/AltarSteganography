using System.Drawing;
using System.Windows.Forms;
using AltarSteganography;
using System.IO;

namespace AltarStenoTest {
	public partial class Form1 : Form {
		private static readonly ImageConverter _imageConverter = new ImageConverter();
		const string Lorem = @"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?";
		const string HalfLorem = @"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet,";
		const string QuarterLorem = @"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo";

		public Form1() {
			InitializeComponent();
			var img = (Bitmap)_imageConverter.ConvertFrom(File.ReadAllBytes("logo-ps.png"));
			TestBox.Image = img;
			
			using (var steno = new BinaryWriter(new Steganograph(img))) {
				steno.Write(Lorem);
			}

			img.Save("logo-ps_revealed.png");

			img = (Bitmap)_imageConverter.ConvertFrom(File.ReadAllBytes("logo-ps_revealed.png"));
			TestBox.Image = img;
					
			using (var steno = new BinaryReader(new Steganograph(img))) {
				MessageBox.Show(steno.ReadString());
			}
		}
	}
}
