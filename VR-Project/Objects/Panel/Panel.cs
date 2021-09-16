using System;
using System.Collections.Generic;

public class Panel
{
	
    public string id { get; set; }

    public Data data { get; set; }

    public class Data
    {
        public string id { get; set; }
        public int width { get; set; }
        public int[][] lines { get; set; }
        public int[] color { get; set; }
        public string text { get; set; }
        public double[] position { get; set; }
        public double size { get; set; }
        public string font { get; set; }
        public string status { get; set; }
        public string image { get; set; }
    }

    public void Clear(string id)
    {
        this.id = "scene/panel/clear";
        this.data = new Data() { id = id };
    }

    public void Swap(string id)
    {
        this.id = "scene/panel/swap";
        this.data = new Data() { id = id };
    }

    public void drawlines(string id, int width, int[][] lines)
    {

        this.id = "scene/panel/drawlines";
        this.data = new Data() { id = id, width = width, lines = lines };
    }

    public void setclearColor(string id, int[] color)
    {

        this.id = "scene/panel/setclearcolor";
        this.data = new Data() { id = id, color = color };
    }

    public void drawText(string id, string text, double[] position, double size, int[] color, string font)
    {
        this.id = "scene/panel/drawtext";
        this.data = new Data() { id = id, text = text, position = position, color = color, font = font };

    }
    //public void image(string id, string image, double[] position, )
    //{

    //}
}
