using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.Barracuda;

public class ImageSelector : MonoBehaviour
{
    // Variables for button
    public Button selectImageButton;
    public Button predictButton;

    // Variable for button
    public RawImage displayBox;

    // Variable to see the result
    public TextMeshProUGUI resultText;

    // Variables of ONNX Net
    public NNModel modelAsset;
    private Model model;
    private IWorker worker;

    // Variable
    private bool isGalleryOpen = false;

    void Start()
    {
        // Inizialization
        resultText.text = "";

        SetButtonText(selectImageButton, "Load Image");
        SetButtonText(predictButton, "Prediction");

        model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);

        selectImageButton.onClick.AddListener(OpenGallery);
        predictButton.onClick.AddListener(PerformPrediction);
    }

    // Method to change the text of a button
    void SetButtonText(Button button, string newText)
    {
        TextMeshProUGUI textComponent = button.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = newText;
        }
        else
        {
            Debug.LogWarning($"The button {button.name} does not have a child TextMeshProUGUI component!");
        }
    }

    // Method to open gallery
    public void OpenGallery()
    {
        if (isGalleryOpen)
        {
            Debug.Log("The gallery is already open, please select an image.");
            return;
        }

        isGalleryOpen = true;
        selectImageButton.interactable = false;

        NativeGallery.GetImageFromGallery((string path) =>
        {
            isGalleryOpen = false;
            selectImageButton.interactable = true;

            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("No images selected.");
                return;
            }

            Debug.Log("Image selected: " + path);

            Texture2D texture = NativeGallery.LoadImageAtPath(path, 1024, false);
            if (texture == null || !texture.isReadable)
            {
                Debug.LogError("The loaded texture is invalid or unreadable.");
                return;
            }

            displayBox.texture = texture;
            resultText.text = "Image uploaded. Click 'Prediction' to run prediction.";
        }, "Select an image");
    }

    // Method to perform the prediction
    public void PerformPrediction()
    {
        if (displayBox.texture != null)
        {
            Texture2D texture = (Texture2D)displayBox.texture;
            StartCoroutine(ClassifyImage(texture));
        }
        else
        {
            resultText.text = "Please upload or take an image before making your prediction!";
        }
    }

    // Function to classify the image
    IEnumerator ClassifyImage(Texture2D texture)
    {
        var inputTensor = PreprocessImage(texture);
        worker.Execute(inputTensor);

        Tensor output = worker.PeekOutput("Logits_softmax"); 
        float[] predictions = output.ToReadOnlyArray();

        
        float benignConfidence = predictions[0];
        float malignantConfidence = predictions[1];
        string prediction = benignConfidence > malignantConfidence ? "Benign" : "Malignant";
        float confidence = Mathf.Max(benignConfidence, malignantConfidence);

        resultText.text = $"Prediction: {prediction} with Confidence: {confidence * 100:F2}%";

        inputTensor.Dispose();
        output.Dispose();
        yield return null;
    }

    // Method to preprocessing image
    Tensor PreprocessImage(Texture2D texture)
    {
        
        Texture2D resizedTexture = ResizeTexture(texture, 224, 224);

        
        Color[] pixels = resizedTexture.GetPixels();
        float[] pixelValues = new float[pixels.Length * 3];

       
        for (int i = 0; i < pixels.Length; i++)
        {
            pixelValues[i * 3] = pixels[i].r;     // Valore del canale R
            pixelValues[i * 3 + 1] = pixels[i].g; // Valore del canale G
            pixelValues[i * 3 + 2] = pixels[i].b; // Valore del canale B
        }

        
        return new Tensor(1, 224, 224, 3, pixelValues);
    }

    // Resize an image
    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(width, height, source.format, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return result;
    }

    void OnApplicationQuit()
    {
        worker.Dispose();
    }
}









