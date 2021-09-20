using System;
using System.IO;

public class Terrain 
{
    public string id { get; set; }
    public TerrainData data { get; set; }

	public Terrain(string id, int[] size, string heightPath)
	{
		this.id = id;

		float baseline = 0;
		float[] localHeigths = new float[65536];
		for (int i = 0; i < 65536; i++)
        {
			Random random = new Random();
			Double number = random.NextDouble();

			
			if (random.Next(100) >= 50)
			{
				baseline += (float)number;
			}
			else
			{
				baseline -= (float)number;
			}

			//baseline mag niet lager zijn dan 60 en niet hoger zijn dan 180.
			baseline = Math.Clamp(baseline, 0, 3);

			localHeigths[i] = baseline;
			//localHeigths[i] = 0f;
        }
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
		public float[]? position { get; set; }
		public float[][]? positions { get; set; }
	}


	private float[] setHeights(String heightPath)
    {
		string fileText = File.ReadAllText(heightPath);

		string[] values = fileText.Split(", ");
		this.data.heights = new float[values.Length];


		for (int i = 0; i < values.Length; i++)
        {
			this.data.heights[i] = int.Parse(values[i]);
        }

		return this.data.heights;
	}

}
