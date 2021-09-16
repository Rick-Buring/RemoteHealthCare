using System;
using System.IO;

public class Terrain : VRObject
{
	private int[] ?size { get; set; }
	private float[] ?heights { get; set; }

	private double[] ?position;
	private double[][] ?positions;



	private string heightPath;

	public Terrain(string id, int[] size, string heightPath)
	{
		this.id = id;
		this.size = size;
        this.heightPath = heightPath;
		this.data = new object[] { this.size, setHeights() };
    }

	private float[] setHeights()
    {
		string fileText = File.ReadAllText(this.heightPath);

		string[] values = fileText.Split(", ");


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
