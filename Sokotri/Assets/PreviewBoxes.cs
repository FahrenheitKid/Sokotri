using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PreviewBoxes : MonoBehaviour
{
   [SerializeField]
    Sprite [] sprites =  new Sprite[6];
    SpriteRenderer spriteRenderer;    // Start is called before the first frame update

    Box.type [] boxes = new Box.type[3];

    [SerializeField]
    GameObject[] boxesRenderers = new GameObject[3];
    void Start()
    {
        if(spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        generateNextPreview();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void updateSprites()
    {
        for(int i = 0; i < boxesRenderers.Length; i++)
        {
            if((int)boxes[i] < 0 || (int)boxes[i] >= sprites.Length || i < 0 || i >= boxes.Length) continue;

            boxesRenderers[i].GetComponent<Image>().sprite = sprites[(int)boxes[i]];
            boxesRenderers[i].GetComponent<Image>().color = (boxes[i] == Box.type.quintessential) ? Box.quintessentialColor : (Color32)Color.white;

        }
    }

    void generateNextPreview()
    {
      for (int i = 0; i < boxes.Length; i++)
      {
          boxes[i] = Box.getRandomBoxType();
      }

      updateSprites();

    }
   
}
