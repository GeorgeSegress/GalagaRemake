using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// short explosion animation script.
public class Explosion : MonoBehaviour
{
    private SpriteRenderer myRenderer;

    public Sprite[] myImages;
    public float countLength;
    private int bkmk = 0;

    private void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Exploding());
    }

    private IEnumerator Exploding()
    {
        yield return null;
        myRenderer.sprite = myImages[0];
        yield return null;
        myRenderer.sprite = myImages[1];
        yield return null;
        myRenderer.sprite = myImages[2];
        yield return null;
        myRenderer.sprite = myImages[3];
        yield return null;
        Destroy(gameObject);
    }
}
