using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ToggleScript : MonoBehaviour {

	public Toggle m_Toggle;
	public Text m_Text;

	// Use this for initialization
	 void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<Toggle>();
        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });

        //Initialise the Text to say the first state of the Toggle
        m_Text.text = "First Value : " + m_Toggle.isOn;
    }

    //Output the new state of the Toggle into Text
    void ToggleValueChanged(Toggle change)
    {
		AudioConfiguration config = AudioSettings.GetConfiguration();
        m_Text.text =  "New Value : " + m_Toggle.isOn;
		if (m_Toggle.isOn) 
		{
            config.speakerMode = AudioSpeakerMode.Mono;
            AudioSettings.Reset(config);
			Debug.Log("I changed to: " + AudioSettings.speakerMode);
		}
		else 
		{
			config.speakerMode = AudioSpeakerMode.Stereo;
			AudioSettings.Reset(config);
			Debug.Log("I changed to: " + AudioSettings.speakerMode);
		}
    }
	// Update is called once per frame
	// void Update () {
		
	// }
}
