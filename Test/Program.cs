using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
			string filePath = @"C:\Users\leonv\Downloads\School\Jaar2\Periode1\Proftaak\RemoteHealthCare\VR-Project\HeightmapSmol.bmp";
			readHeight(filePath);
        }

		public static void readHeight(string filePath)
		{

			List<float> list = new List<float>();
			int number = 0;

            Bitmap img = new Bitmap(filePath);
			for (int i = 0; i < img.Height; i++)
			{
				for (int j = 0; j < img.Width; j++)
				{
					Color pixel = img.GetPixel(i, j);
					//list.Add(pixel.GetBrightness());
					Console.WriteLine("Adding : " + number + " : " + (pixel.GetBrightness() * 10f));
					number++;
				}
			}

		}

	}
}
