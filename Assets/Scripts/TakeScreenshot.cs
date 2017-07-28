/*
Script made by: Mathijs van Sambeek
For more tutorials: https://www.youtube.com/user/0Imagineer0?sub_confirmation=1

This script can be used for free if you give credits to the maker of the script.
*/

using UnityEngine;
using System.Collections;

public class TakeScreenshot : MonoBehaviour {
 
 public bool controlable = false;
 public float moveSpeed = 1;
 public int resolution = 3; // 1= default, 2= 2x default, etc.
 public string imageName = "Screenshot_";
 public string customPath = "C:/Users/default/Desktop/UnityScreenshots/"; // leave blank for project file location
 public bool resetIndex = false;
 
 private int index = 0;
 
 void Awake()
 {
  if(resetIndex) PlayerPrefs.SetInt("ScreenshotIndex", 0);
  if(customPath != "")
  {
   if(!System.IO.Directory.Exists(customPath))
   {
    System.IO.Directory.CreateDirectory(customPath);
   }
  }
  index = PlayerPrefs.GetInt("ScreenshotIndex") != 0 ? PlayerPrefs.GetInt("ScreenshotIndex") : 1;
 }
 
 void Update()
 {
  if(controlable)
  {
   if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
   {
    transform.Translate(0,0,(moveSpeed*Time.deltaTime) * 4);
   }
   else if(Input.GetKey(KeyCode.W))
   {
    transform.Translate(0,0,moveSpeed*Time.deltaTime);
   }
   if(Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))
   {
    transform.Translate(0,0,-moveSpeed*Time.deltaTime * 4);
   }
   else if(Input.GetKey(KeyCode.S))
   {
    transform.Translate(0,0,-moveSpeed*Time.deltaTime);
   }
   if(Input.GetKey(KeyCode.A))
   {
    transform.Translate(-moveSpeed*Time.deltaTime, 0, 0);
   }
   else if(Input.GetKey(KeyCode.D))
   {
    transform.Translate(moveSpeed*Time.deltaTime, 0, 0);
   }
  }
 }
 
 void LateUpdate () 
  {
    if (Input.GetButtonDown("Take Screenshot"))
    {
    ScreenCapture.CaptureScreenshot(customPath + imageName + index + ".png", resolution);
    index++;
    Debug.LogWarning("Screenshot saved: " + customPath + " --- " + imageName + index);
  }
 }
 
 void OnApplicationQuit()
 {
  PlayerPrefs.SetInt("ScreenshotIndex", (index));
 }
}