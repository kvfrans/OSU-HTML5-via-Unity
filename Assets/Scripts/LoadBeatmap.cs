using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadBeatmap : MonoBehaviour {

	public string filelocation;
	public string beatmapname;

	private List<string> commands;

	private List<Beat> beats;

	public Transform pointbeat;

	public Transform background;

	// Use this for initialization
	void Start () {

		beats = new List<Beat>();

		commands = GetComponent<ReadFile>().Load(filelocation+"/"+beatmapname);

		int songindex = commands.FindIndex(x => x.StartsWith("AudioFilename"));
		string songloc = commands[songindex].Substring(15);
		string songraw = songloc.Split("."[0])[0];
		Debug.Log(songraw);
		GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(filelocation+"/"+songraw) as AudioClip;
		GetComponent<AudioSource>().Play();

		int imageindex = commands.FindIndex(x => x.StartsWith("0,0,"));
		string imageloc = commands[imageindex].Substring(5);
		string imageraw = imageloc.Split("."[0])[0];
		Debug.Log(imageraw);
		Texture2D tex = Resources.Load<Texture2D>(filelocation+"/"+imageraw) as Texture2D;
		background.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(0.5f,0.5f));
		// background.GetComponent<Spr
		// GetComponent<AudioSource>().Play();

		int beatsindex = commands.FindIndex(x => x.StartsWith("[HitObjects]"));
		for(int i = beatsindex+1; i < commands.Count; i++)
		{
			string line = commands[i];
			string[] components = line.Split(',');
			// Debug.Log(line);
			string type = "point";
			int repeats = 0;
			List<Vector2> path = new List<Vector2>();
			if(components.Length > 6)
			{
				type = "slider";
				string typeandsliders = components[5];
				string[] tasComponents = typeandsliders.Split('|');
				repeats = int.Parse(components[6]);
				for(int x = 1; x < tasComponents.Length; x++)
				{
					path.Add(new Vector2(float.Parse(tasComponents[x].Split(':')[0]),float.Parse(tasComponents[x].Split(':')[1])));
				}
			}
			Beat beat = new Beat(new Vector2(int.Parse(components[0]),int.Parse(components[1])),type,path,int.Parse(components[2]),repeats);
			beats.Add(beat);
		}


	}

	// Update is called once per frame
	void Update () {
		Beat currentbeat = beats[0];
		if(Time.time*1000 >= currentbeat.time - 1500)
		{
			var localbeat = Instantiate(pointbeat);
			localbeat.position = currentbeat.location;
			localbeat.GetComponent<Hit>().beat = currentbeat;
			beats.Remove(currentbeat);
		}
	}

	public class Beat
	{
		public Vector2 location;
		public string type;
		public List<Vector2> path;
		public int time;
		public int repeats;

	    public Beat(Vector2 loc, string typ, List<Vector2> pat, int tim, int rep)
	    {
	        location = loc;
	        type = typ;
	        path = pat;
	        time = tim;
	        repeats = rep;
	    }
	}
}
