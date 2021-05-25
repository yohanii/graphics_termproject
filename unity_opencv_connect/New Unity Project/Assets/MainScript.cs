using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct CvRect
{
    public int x, y, w, h;
}

public class MainScript : MonoBehaviour
{
    public GameObject imgObj;
    private SpriteRenderer sr;
    public Sprite spr2;
    private int cur_frame_pos, last_frame_pos;
    private CvRect fr;
    private int cam_width, cam_height, cam_fps;
    private bool cam_ready;
    private float[] rot_angles;
    private int rot_len;
    private float timer = 0.0f;
    private float wait_time = 0.1f;
    private Vector2 pers_ratio;
    private Vector3 cam_pos;
    [DllImport("OpenCVDLL_face")]
    private static extern void FlipImage(ref Color32[] rawImage, int width, int height);


    [DllImport("OpenCVDLL_face")]
    private static extern int init_capture(string face_cascade_file, ref int cam_width, ref int cam_height, ref int cam_fps);
    

    [DllImport("OpenCVDLL_face")]
    private static extern void get_frame_pos(ref int cam_frame_pos);

    [DllImport("OpenCVDLL_face")]
    private static extern void detect_rect(ref CvRect fr, float[] rot_angles, int rot_len, int width, int height, int camVis);

    [DllImport("OpenCVDLL_face")]
    private static extern void close_capture(); 

    // Start is called before the first frame update
    void Start()
    {
        string get_env = System.Environment.GetEnvironmentVariable("OPENCV_DIR");
        string face_cascade_file = get_env + "\\data\\haarcascades\\haarcascade_frontalface_alt.xml";
        sr = imgObj.GetComponent<SpriteRenderer>();
        int res;
        cam_ready = false;
        res = init_capture(face_cascade_file, ref cam_width, ref cam_height, ref cam_fps);
        if (res < 0)
        {
            Debug.LogWarningFormat("[{0}] Failed .", GetType());
            return;
        }
        last_frame_pos = -1;
        rot_angles = new float[] { 0.0f };
        rot_len = 1;
        cam_ready = true;
        timer = 0.0f;
        wait_time = 0.2f;
        // cam_obj = this.gameObject;
        pers_ratio = new Vector2(0.05f, 0.05f);
        cam_pos = this.transform.position;
    }
    void OnApplicationQuit()
    {
        close_capture();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            var imgPixels = sr.sprite.texture.GetPixels32();
            //var imgSprite = imgObj.sprite.texture.GetPixels32();
            FlipImage(ref imgPixels, 400, 400);

            Texture2D tex1 = new Texture2D(400, 400);
            tex1.SetPixels32(imgPixels);
            tex1.Apply();
            sr.sprite = Sprite.Create(tex1, new Rect(0, 0, tex1.width, tex1.height), new Vector2(0.5f, 0.5f), 100);
        }
        
        if (cam_ready)
        {
            timer += Time.deltaTime;
            //Debug.Log("time : " + timer);
            if (timer > wait_time)
            {
                timer = timer - wait_time;
                detect_rect(ref fr, rot_angles, rot_len, cam_width, cam_height, 1);
                //Debug.Log("frame_rect : " + fr.x + ", " + fr.y + ", " + fr.w + ", " + fr.h);
                if (fr.w != 0)
                {
                    double cx = fr.x + fr.w * 0.5;
                    double cy = fr.y + fr.h * 0.5;
                    double newx = (cam_width * 0.5 - cx) * pers_ratio.x + cam_pos.x;
                    double newy = (cam_height * 0.5 - cy) * pers_ratio.y + cam_pos.y;
                    this.transform.position = new Vector3((float)newx, (float)newy, cam_pos.z);
                }
            }
            /*get_frame_pos(ref cur_frame_pos);
            Debug.Log("current frame pos : " + cur_frame_pos);
            if (cur_frame_pos > last_frame_pos)
            {
                
                detect_rect(ref fr, rot_angles, rot_len, cam_width, cam_height, 1);
                last_frame_pos = cur_frame_pos;
            }*/
        }
    }
}
