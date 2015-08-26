using UnityEngine;
using System.Collections;

public class Hit : MonoBehaviour {

	public Color maincolor;

	public Transform bg;
	public Transform border;
	public Transform approach;
	public Transform ballbg;

	public Transform fakehit;
	public Transform fakehit2;

	public LoadBeatmap.Beat beat;

	private float lifetime = 3;

	public float speed = 10;
  	public Vector2 direction = new Vector2(0, 0);

  	private Vector2 movement;

  	private bool dead = false;
  	private float fadeout = 1;

  	// publ

  	private bool moving = false;
  	private bool reverse = false;

	// Use this for initialization
	void Start () {
		bg.GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
		approach.GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
		border.GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
		fakehit2.Find("bg").GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
		fakehit2.Find("border").GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
		fakehit.Find("bg").GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);
		fakehit.Find("border").GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);

		if(beat.type == "slider")
		{
			GetComponent<LineRenderer>().enabled = true;
			GetComponent<LineRenderer>().SetPosition(0,beat.location);
			GetComponent<LineRenderer>().SetPosition(1,beat.path[beat.path.Count -1]);
			fakehit.position = beat.path[beat.path.Count-1];
			fakehit2.position = beat.location;
		}

	}

	// Update is called once per frame
	void Update () {

		if(beat.type == "slider")
		{
			fakehit.position = beat.path[beat.path.Count-1];
			fakehit2.position = beat.location;
		}
		// Debug.Log(beat.repeats);
		// Debug.Log(reverse);

		if(lifetime > 2f)
		{
			GetComponent<LineRenderer>().SetColors(new Color(maincolor.r,maincolor.b,maincolor.g,(3f-lifetime)/1f),new Color(maincolor.r,maincolor.b,maincolor.g,(3f-lifetime)/1f));
			bg.GetComponent<SpriteRenderer>().color = new Color(maincolor.r,maincolor.b,maincolor.g,(3f-lifetime)/1f);
			border.GetComponent<SpriteRenderer>().color = new Color(255,255,255,(3f-lifetime)/1f);
			// approach.GetComponent<SpriteRenderer>().color = new Color(maincolor.r,maincolor.b,maincolor.g,(3f-lifetime)/1f);
			fakehit2.Find("bg").GetComponent<SpriteRenderer>().color = new Color(255,255,255,(3f-lifetime)/1f);
			fakehit2.Find("border").GetComponent<SpriteRenderer>().color = new Color(255,255,255,(3f-lifetime)/1f);
			// fakehit.Find("bg").GetComponent<SpriteRenderer>().color = new Color(255,255,255,(3f-lifetime)/1f);
			fakehit.Find("border").GetComponent<SpriteRenderer>().color = new Color(255,255,255,(3f-lifetime)/1f);

		}
		else
		{
			bg.GetComponent<SpriteRenderer>().color = maincolor;
			if(border != null)
				border.GetComponent<SpriteRenderer>().color = new Color(255,255,255,1);
			if(approach != null)
				approach.GetComponent<SpriteRenderer>().color = maincolor;
		}
		lifetime -= Time.deltaTime*2;
		if(approach != null)
		{
			approach.localScale = new Vector3(lifetime+1,lifetime+1,lifetime+1);
		}
		if(lifetime <= 0)
		{
			if(beat.type == "point")
			{
				GetComponent<Animator>().Play( Animator.StringToHash( "Hit" ) );
				dead = true;
			}
			else if(!moving)
			{
				moving = true;
				Destroy(approach.gameObject);
				Destroy(border.gameObject);
				bg.GetComponent<Animator>().Play( Animator.StringToHash( "Ball" ) );
				ballbg.GetComponent<SpriteRenderer>().enabled = true;
			}



		}

		if(moving)
		{
			Vector2 target = beat.path[beat.path.Count - 1];
			// Debug.Log(target);

			if(reverse)
			{
				target = beat.location;
			}

			direction = target - new Vector2(transform.position.x,transform.position.y);
			if(direction.sqrMagnitude < 25)
			{
				if(beat.repeats > 1)
				{
					beat.repeats--;
					reverse = !reverse;
				}
				else
				{
					dead = true;
				}
			}

			direction.Normalize();


		}

		movement = new Vector2(direction.x * speed,direction.y * speed);


		if(dead)
		{

			fadeout -= Time.deltaTime * 3;

			GetComponent<LineRenderer>().SetColors(new Color(maincolor.r,maincolor.b,maincolor.g,fadeout),new Color(maincolor.r,maincolor.b,maincolor.g,fadeout));
			bg.GetComponent<SpriteRenderer>().color = new Color(maincolor.r,maincolor.b,maincolor.g,fadeout);
			if(border != null)
				border.GetComponent<SpriteRenderer>().color = new Color(255,255,255,fadeout);
			// approach.GetComponent<SpriteRenderer>().color = new Color(maincolor.r,maincolor.b,maincolor.g,(3f-lifetime)/1f);
			fakehit2.Find("bg").GetComponent<SpriteRenderer>().color = new Color(255,255,255,fadeout);
			fakehit2.Find("border").GetComponent<SpriteRenderer>().color = new Color(255,255,255,fadeout);
			// fakehit.Find("bg").GetComponent<SpriteRenderer>().color = new Color(255,255,255,(3f-lifetime)/1f);
			fakehit.Find("border").GetComponent<SpriteRenderer>().color = new Color(255,255,255,fadeout);

			if(fadeout < 0)
			{
				Destroy(gameObject);
			}

			movement = new Vector2(0,0);
		}


		if(Input.GetMouseButton(0))
			{
				checkTouch(Input.mousePosition);
			}


	}

	void FixedUpdate()
  	{
    // Apply movement to the rigidbody
    	GetComponent<Rigidbody2D>().velocity = movement;
  	}

  	void checkTouch(Vector2 pos)
	{
	    Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
    	Vector2 touchPos = new Vector2(wp.x, wp.y);
    	var hit = Physics2D.OverlapPoint(touchPos);
    	if(hit)
    	{
    		if(beat.type == "point")
			{
				GetComponent<Animator>().Play( Animator.StringToHash( "Hit" ) );
				dead = true;

				float score = 0;
				if(lifetime < 0.5)
				{
					score = 300;
				}
				else if(lifetime < 1.5)
				{
					score = 100;
				}
				else if(lifetime < 2.5)
				{
					score = 50;
				}
				Debug.Log(score);

			}
    	}
	}


}
