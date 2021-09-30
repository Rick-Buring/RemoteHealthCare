using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class Terrain 
{
    public string id { get; set; }
    public TerrainData data { get; set; }

	public Terrain(string id, int[] size, string heightPath)
	{
		this.id = id;

		float[] localHeigths = readHeight(heightPath);
		this.data = new TerrainData(size, localHeigths);
    }

	public Terrain(string id, float[] position, float[][] positions)
	{
		this.id = id;
		this.data = new TerrainData(position, positions);
	}

	public class TerrainData
	{

		public TerrainData(int[] size, float[] heights)
        {
            this.size = size;
            this.heights = heights;
        }

		public TerrainData(float[] position, float[][] positions)
		{
			this.position = position;
			this.positions = positions;
		}

		public int[] size { get; set; }
		public float[] heights { get; set; }
#nullable enable
		public float[]? position { get; set; }
		public float[][]? positions { get; set; }
#nullable disable
	}


	public float[] readHeight(string filePath)
	{

		List<float> list = new List<float>();
		int number = 0;

		Bitmap img = new Bitmap(filePath);
		for (int i = 0; i < img.Width; i++)
		{
			for (int j = 0; j < img.Height; j++)
			{
				Color pixel = img.GetPixel(i, j);
				list.Add(pixel.GetBrightness() * 10f);
				//Console.WriteLine("Adding : " + number + " : " + pixel.GetBrightness());
				number++;
			}
		}

		return list.ToArray();

	}

}
