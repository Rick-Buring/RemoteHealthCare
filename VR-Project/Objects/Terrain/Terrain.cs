using System;
using System.IO;

public class Terrain 
{
    public string id { get; set; }
    public TerrainData data { get; set; }
    public int[] size { get; set; }
	public float[] ?heights { get; set; }

	public double[] ?position;
	public double[][] ?positions;



	public string heightPath;

	public Terrain(string id, int[] size, string heightPath)
	{
		this.id = id;
		this.size = size;
        this.heightPath = heightPath;
		this.data = new TerrainData(this.size, setHeights());
    }

	public class TerrainData
	{
        public TerrainData(int[] size, float[] heightPath)
        {
            this.size = size;
            this.heightPath = heightPath;
        }

        public int[] size { get; set; }
		public float[] heightPath { get; set; }
	}


	private float[] setHeights()
    {
		string fileText = File.ReadAllText(this.heightPath);

		string[] values = fileText.Split(", ");
		this.heights = new float[values.Length];


		for (int i = 0; i < values.Length; i++)
        {
			this.heights[i] = float.Parse(values[i]);
        }

		return this.heights;
	}

	//__TODO__ Verstuur: scene/terrain/update
	public void update()
    {

    }

	//__TODO__ Verstuur: scene/terrain/delete 
	public void delete()
    {

    }


	//__TODO__ Verstuur: scene/terrain/getheight
	public void getHeight(double[] ?position, double[][] ?positions)
    {

		this.position = position;
		this.positions = positions;

    }

}
