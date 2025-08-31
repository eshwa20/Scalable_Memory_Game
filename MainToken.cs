using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainToken : MonoBehaviour
{
    GameObject gameControl;
    SpriteRenderer spriteRenderer;
    public Sprite[] faces;
    public Sprite back;
    public int faceIndex;
    public bool matched = false;

    public void OnMouseDown()
    {
        if (matched == false)
        {
            GameControl controlScript = gameControl.GetComponent<GameControl>();

            if (spriteRenderer.sprite == back)
            {
                if (controlScript.TokenUp(this))
                {
                    spriteRenderer.sprite = faces[faceIndex];
                    controlScript.CheckTokens();
                }
            }
        }
    }

    private void Awake()
    {
        gameControl = GameObject.Find("GameControl");
        if (gameControl == null)
        {
            GameControl ctrl = FindObjectOfType<GameControl>();
            if (ctrl != null)
                gameControl = ctrl.gameObject;
            else
                Debug.LogError("GameControl not found in scene. Please add a GameObject named 'GameControl' with the GameControl script.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FlipToFront()
    {
        spriteRenderer.sprite = faces[faceIndex];
    }

    public void FlipToBack()
    {
        if (!matched)
        {
            spriteRenderer.sprite = back;
        }
    }

}
