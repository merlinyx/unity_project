using System.IO;
using Emgu.CV.CvEnum;
using UnityEngine;
using System;
using System.Drawing;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.OCR;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

public class Ocr : MonoBehaviour {

    private static System.Drawing.Font font;
    private static System.Drawing.Color black;
    public float fontsize;

    private GameObject ivy;
    private Material ivy_mat;
    private Material ocr_mat;
    private bool isIvy = true;

    private Tesseract _ocr;
    private List<Region_map> word_region_map = new List<Region_map>();

    private string word = "";
    private Texture2D original_texture;

    private void Awake() {
        original_texture = (Texture2D) GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        ocr_mat = new Material(Shader.Find("Standard"));

        ocr_mat.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        ocr_mat.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ocr_mat.SetInt("_ZWrite", 0);
        ocr_mat.DisableKeyword("_ALPHATEST_ON");
        ocr_mat.EnableKeyword("_ALPHABLEND_ON");
        ocr_mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        ocr_mat.renderQueue = 3000;

        ivy = GameObject.FindWithTag("Ivy");
        if (ivy == null) {
            Debug.Log("No RT Ivy in scene!");
        } else {
            ivy_mat = ivy.GetComponent<MeshRenderer>().sharedMaterials[1];
            Debug.Log("Ivy mat: " + ivy_mat.name);
        }
        font = new System.Drawing.Font(new FontFamily("Arial"), fontsize);
        black = System.Drawing.Color.FromName("Black");
    }

    // Use this for initialization
    void Start() {
        String[] names = new string[] { "eng.cube.bigrams", "eng.cube.fold", "eng.cube.lm", "eng.cube.nn", "eng.cube.params", "eng.cube.size", "eng.cube.word-freq", "eng.tesseract_cube.nn", "eng.traineddata" };

        String outputPath = Path.Combine("C:\\Emgu/emgucv-windesktop 3.1.0.2504/Emgu.CV.World", "tessdata");
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        foreach (String n in names) {
            TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine("tessdata", n));
            String filePath = Path.Combine(outputPath, n);
#if UNITY_METRO
           UnityEngine.Windows.File.WriteAllBytes(filePath, textAsset.bytes);
#else
            if (!File.Exists(filePath))
                File.WriteAllBytes(filePath, textAsset.bytes);
#endif
        }

        _ocr = new Tesseract(outputPath, "eng", OcrEngineMode.TesseractCubeCombined);

        Debug.Log("OCR engine loaded.");
        print("OCR processing..");

        Image<Bgr, Byte> img = TextureConvert.Texture2dToImage<Bgr, Byte>(original_texture);
        _ocr.Recognize(img);

        Tesseract.Character[] characters = _ocr.GetCharacters();
        foreach (Tesseract.Character c in characters) { //draw rect for each character
            CvInvoke.Rectangle(img, c.Region, new MCvScalar(255, 0, 0));
        }

        String messageOcr = _ocr.GetText().TrimEnd('\n', '\r'); // remove end of line from ocr-ed text   
        Debug.Log("Detected text: " + messageOcr);

        Texture2D texture = TextureConvert.InputArrayToTexture2D(img, FlipType.Vertical);
        original_texture = texture;
        build_map(characters);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit)) {
                Vector2 hit_coord = hit.textureCoord; //get the hit uv-coordinate on the texture
                Debug.Log("Hit coord: " + hit_coord);
                float texture_x = hit_coord.x * original_texture.width;
                float texture_y = (1 - hit_coord.y) * original_texture.height;
                Vector2 texture_point = new Vector2(texture_x, texture_y);
                Debug.Log("Texture coord: " + texture_x + ", " + texture_y);
                foreach (Region_map map in word_region_map) {
                    if (within_region(texture_point, map.getRect())) {
                        word = map.getWord();
                        Debug.Log("Clicking on " + word);
                        //StartCoroutine(CaptureTextArea());
                        String path = "Assets/Resources/" + word + ".png";
                        StartCoroutine(DrawText(word, 120, path));
                        Debug.Log("image at path " + path);
                        break;
                    }
                }
            }
        } else if (Input.GetKeyDown(KeyCode.S)) {
            Texture t = Resources.Load(word) as Texture;
            if (t != null) {
                ocr_mat.mainTexture = t;
                ivy.GetComponent<RTIvyController>().SendLeaveMaterial(ocr_mat);
                isIvy = false;
            }
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            if (isIvy) {
                ivy.GetComponent<RTIvyController>().SendLeaveMaterial(ivy_mat);
            } else {
                ivy.GetComponent<RTIvyController>().SendLeaveMaterial(ocr_mat);
            }
            isIvy = !isIvy;
        }
    }

    ///// https://answers.unity.com/answers/514209/view.html
    //IEnumerator CaptureTextArea() {
    //    //Run at the end of frame so we get the most recent data
    //    //(otherwise Unity kicks up a fuss about buffers).
    //    yield return new WaitForEndOfFrame();

    //    int width = Mathf.FloorToInt(TextAreaRect.width);
    //    int height = Mathf.FloorToInt(TextAreaRect.height);
    //    Texture2D text_texture = new Texture2D(width, height, TextureFormat.RGB24, false);

    //    Rect tempRect = TextAreaRect;
    //    //There's probably a tidier way to calculate this:
    //    //Flip to match the Y axis properly.
    //    tempRect.y = Screen.height - TextAreaRect.y - TextAreaRect.height;

    //    //Grab the pixels from the system buffer.
    //    text_texture.ReadPixels(tempRect, 0, 0);
    //    text_texture.Apply(); //Apply the read.
    //    ocr_mat.mainTexture = text_texture;
    //    ivy.GetComponent<RTIvyController>().SendLeaveMaterial(ocr_mat);
    //    isIvy = false;
    //}

    /// <summary>
    /// Converting text to image (png).
    /// </summary>
    /// <param name="text">text to convert</param>
    /// <param name="font">Font to use</param>
    /// <param name="path">path to save the image</param>
    static IEnumerator DrawText(String text, int maxWidth, String path) {
        yield return new WaitForEndOfFrame();

        //first, create a dummy bitmap just to get a graphics object
        Image img = new Bitmap(1, 1);
        System.Drawing.Graphics drawing = System.Drawing.Graphics.FromImage(img);
        //measure the string to see how big the image needs to be
        SizeF textSize = drawing.MeasureString(text, font, maxWidth);

        //set the stringformat flags to rtl
        StringFormat sf = new StringFormat();
        //uncomment the next line for right to left languages
        //sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
        sf.Trimming = StringTrimming.Word;
        //free up the dummy image and old graphics object
        img.Dispose();
        drawing.Dispose();

        //create a new image of the right size
        img = new Bitmap((int)textSize.Width, (int)textSize.Height);

        drawing = System.Drawing.Graphics.FromImage(img);
        //Adjust for high quality
        drawing.CompositingQuality = CompositingQuality.HighQuality;
        drawing.InterpolationMode = InterpolationMode.HighQualityBilinear;
        drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
        drawing.SmoothingMode = SmoothingMode.HighQuality;
        drawing.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        //paint the background
        drawing.Clear(System.Drawing.Color.Transparent);

        //create a brush for the text
        Brush textBrush = new SolidBrush(black);
        Debug.Log("drawing...");

        drawing.DrawString(text, font, textBrush, new RectangleF(0, 0, textSize.Width, textSize.Height), sf);
        drawing.Save();

        textBrush.Dispose();
        drawing.Dispose();
        img.Save(path, ImageFormat.Png);
        Debug.Log("done!");
        img.Dispose();
    }

    private bool within_region(Vector2 point, Rectangle rect) {
        return (point.x >= rect.Left && point.x <= rect.Right && point.y >= rect.Top && point.y <= rect.Bottom);
    }

    private void build_map(Tesseract.Character[] chars) { // build hashmap for words and regions
        string word = "";
        Rectangle rect = new Rectangle();
        for (int i = 0; i < chars.Length; i++) {
            int c = Convert.ToInt32((chars[i].Text)[0]);
            if ((c >= 65 & c <= 90) | (c >= 97 & c <= 122)) {// is character a-z/A-Z
                // part of a word
                word += chars[i].Text;
                if (rect.IsEmpty)
                    rect = chars[i].Region;
                else
                    rect.Width += chars[i].Region.Width;
            } else if (!word.Equals("") && !rect.IsEmpty) { //end of a word
                if (_ocr.IsValidWord(word) != 0) {
                    word_region_map.Add(new Region_map(rect, word));
                }
                //Debug.Log(word + ": " + rect.ToString());
                //if (_ocr.IsValidWord(word) != 0) Debug.Log("Valid word");
                rect = new Rectangle();
                word = "";
            }
        }
        word_region_map.Sort();
    }
}

//region-word map
public class Region_map : IComparable<Region_map> {

    private Rectangle rect;
    private string word;

    public Region_map(Rectangle rect, string word) {
        this.rect = rect;
        this.word = word;
    }

    public int CompareTo(Region_map other) { // partial sort
        if (rect.Y > other.rect.Y)
            return 1;
        else if (rect.X == other.rect.X && rect.Y == other.rect.Y)
            return 0;
        else
            return -1;
    }

    public Rectangle getRect() {
        return rect;
    }

    public string getWord() {
        return word;
    }
}
