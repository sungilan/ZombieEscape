using UnityEngine;
          using System.Collections;
          using UnityEngine.UI;
          using UnityEngine.EventSystems;
          
          public class TextBlink : MonoBehaviour {

          	Text flashingText;
          	// Use this for initialization 
          	void Start () {
          		flashingText = GetComponent<Text> ();       
          		StartCoroutine (BlinkText());
          	}
        
          	public IEnumerator BlinkText(){
          		while (true) {
          			flashingText.text = "";
          			yield return new WaitForSeconds (.5f);
          			flashingText.text = "Fress Enter";       
          			yield return new WaitForSeconds (.5f);
          		}
          	}
          }