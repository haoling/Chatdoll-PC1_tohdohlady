using UnityEngine;
using Cysharp.Threading.Tasks;
using ChatdollKit.Dialog;
using ChatdollKit.IO;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WhisperRestVoiceRequestProvider : VoiceRequestProviderBase
{
    [Header("Whisper Rest Server Settings")]
    public string EndpointUrl = string.Empty;
    public string Language = "ja";
    public string initialPrompt = string.Empty;

    protected override async UniTask<string> RecognizeSpeechAsync(VoiceRecorderResponse recordedVoice)
    {
        if (string.IsNullOrEmpty(EndpointUrl) || string.IsNullOrEmpty(Language))
        {
            Debug.LogError("Url or Language are missing from WhisperRestVoiceRequestProvider");
        }

        string callUrl = EndpointUrl.Trim('/') + $"/asr?task=transcribe&language={Language}&encode=true&output=json";
        if (!string.IsNullOrEmpty(initialPrompt))
        {
            callUrl += $"&initial_prompt={initialPrompt}";
        }

        byte[] boundary = UnityWebRequest.GenerateBoundary();
        List<IMultipartFormSection> data = new List<IMultipartFormSection>();
        MultipartFormFileSection section = new MultipartFormFileSection("audio_file", AudioConverter.AudioClipToPCM(recordedVoice.Voice, recordedVoice.SamplingData), "audio.wav", "audio/wav");
        data.Add(section);
        byte[] formSections = UnityWebRequest.SerializeFormSections(data, boundary);

        var response = await client.PostBytesAsync<SpeechRecognitionResponse>(
            callUrl,
            formSections,
            new Dictionary<string, string>() { { "Content-Type", $"multipart/form-data; boundary={System.Text.Encoding.UTF8.GetString(boundary, 0, boundary.Length)}" } }
        );

        var recognizedText = response?.text ?? string.Empty;
        return recognizedText;
    }

    // Models for response
    class SpeechRecognitionResponse
    {
        public string text;
        public SpeechRecognitionSegment[] segments;
        public string language;
    }

    class SpeechRecognitionSegment
    {
        public int id;
        public int seek;
        public double start;
        public double end;
        public string text;
        public int[] tokens;
        public double temperature;
        public double avg_logprob;
        public double compression_ratio;
        public double no_speech_prob;
    }
}
