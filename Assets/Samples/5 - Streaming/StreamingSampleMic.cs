using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;

namespace Whisper.Samples
{
    /// <summary>
    /// Stream transcription from microphone input.
    /// </summary>
    public class StreamingSampleMic : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;

        public List<Transform> locations;
        public float speed;

        private Transform goal;
    
        [Header("UI")] 
        public Button button;
        public Text buttonText;
        public Text text;
        public ScrollRect scroll;
        private WhisperStream _stream;

        private async void Start()
        {
            goal = transform;
            _stream = await whisper.CreateStream(microphoneRecord);
            _stream.OnResultUpdated += OnResult;
            _stream.OnSegmentUpdated += OnSegmentUpdated;
            _stream.OnSegmentFinished += OnSegmentFinished;
            _stream.OnStreamFinished += OnFinished;

            microphoneRecord.OnRecordStop += OnRecordStop;
            button.onClick.AddListener(OnButtonPressed);
        }

        private void Update()
        {
            if(text.text.IndexOf("TRASH CAN", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (text.text.IndexOf("go", StringComparison.OrdinalIgnoreCase) >= 0|| text.text.IndexOf("come", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foreach (Transform location in locations)
                    {
                        if (text.text.IndexOf(location.name, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            goal = location;
                        }
                    }
                }  
                

            }     
            
            MoveTo(goal);
        }

        private void OnButtonPressed()
        {
            if (!microphoneRecord.IsRecording)
            {
                _stream.StartStream();
                microphoneRecord.StartRecord();
            }
            else
                microphoneRecord.StopRecord();
        
            buttonText.text = microphoneRecord.IsRecording ? "Stop" : "Record";
        }
    
        private void OnRecordStop(AudioChunk recordedAudio)
        {
            buttonText.text = "Record";
        }
    
        private void OnResult(string result)
        {
            text.text = result;
            UiUtils.ScrollDown(scroll);
        }
        
        private void OnSegmentUpdated(WhisperResult segment)
        {
            print($"Segment updated: {segment.Result}");
        }
        
        private void OnSegmentFinished(WhisperResult segment)
        {
            print($"Segment finished: {segment.Result}");
        }
        
        private void OnFinished(string finalResult)
        {
            print("Stream finished!");
        }

        private void MoveTo(Transform _goal)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_goal.position.x,transform.position.y, _goal.position.z), speed * Time.deltaTime);
        }
    }
}
