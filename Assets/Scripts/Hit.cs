using UnityEngine;
using System.Collections;

public class Hit : MonoBehaviour {

	public Color maincolor;

	public Transform bg;
	public Transform border;
	public Transform approach;

	private float lifetime = 3;

	// Use this for initialization
	void Start () {
		bg.GetComponent<SpriteRenderer>().color = maincolor;
		approach.GetComponent<SpriteRenderer>().color = maincolor;
	}

	// Update is called once per frame
	void Update () {
		lifetime -= Time.deltaTime*2;
		approach.localScale = new Vector3(lifetime+1,lifetime+1,lifetime+1);
	}
}
