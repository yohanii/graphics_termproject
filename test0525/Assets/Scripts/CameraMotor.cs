using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct CvRect
{
    public int x, y, w, h;
}
public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;
    private Vector3 startOffset;
    private Vector3 moveVector;

    private float transition = 0.0f;
    private float animationDuration = 3.0f;
    private Vector3 animationOffset = new Vector3(0.0f, 3.5f, 3.0f);

    private Vector3 last_offset;
    public float rot_transition = 2.0f;
    private float rotationDuration = 1.0f;

    private int cur_frame_pos, last_frame_pos = -1;
    private CvRect fr;
    private int cam_width, cam_height, cam_fps;
    private bool cam_ready = true;
    private float[] rot_angles = new float[] { 0.0f };
    private int rot_len = 1;
    private float timer = 0.0f;
    private float wait_time = 0.3f;
    private Vector3 pers_ratio = new Vector3(0.02f, 0.02f, 0.02f);
    private Vector3 cam_pos;
    private float face_cx, face_cy, face_width, face_height;

    public float dx, dy, dz;
    private float rho;
    private float theta, phi;
    private float start_angle;

    public int res;
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
        //string get_env = System.Environment.GetEnvironmentVariable("OPENCV_DIR");
        //string get_env = "C:\\opencv_build";
        //string face_cascade_file = get_env + "\\data\\haarcascades\\haarcascade_frontalface_alt.xml";
        string face_cascade_file = Application.streamingAssetsPath + "\\data\\haarcascades\\haarcascade_frontalface_alt.xml";
        face_cx = cam_width * 0.5f;
        face_cy = cam_height * 0.5f;

        lookAt = GameObject.FindGameObjectWithTag("Player").transform;
        startOffset = transform.position - lookAt.position;
        start_angle = Vector3.Angle(Vector3.back, startOffset);
        last_offset = startOffset;
        dx = 0.0f;
        dy = 0.0f;
        dz = 0.0f;
        rho = 3.0f;


        //rot_angles = new float[] { 0.0f, -25.0f, 25.0f };
        //rot_len = 3;

        res = init_capture(face_cascade_file, ref cam_width, ref cam_height, ref cam_fps);
        if (res < 0)
        {
            Debug.LogWarningFormat("[{0}] Failed .", GetType());
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (res >= 0)
        {
            get_face_pos();
        }
        Vector3 cur_offset = Quaternion.Euler(lookAt.eulerAngles) * startOffset;
        //Vector3 offset_interpolate = last_offset;

        //Debug.Log("diff : " + Vector3.Angle(cur_offset, last_offset));
        if (rot_transition > 1.0f && Vector3.Angle(cur_offset, last_offset) > 10.0f)
        {
            rot_transition = 0.0f;
        }

        if (rot_transition <= 1.0f)
        {
            Vector3 offset_interpolate = Vector3.Slerp(last_offset, cur_offset, rot_transition);
            //Debug.Log("last : " + last_offset + " cur : " + cur_offset + " interpolate : " + offset_interpolate);
            moveVector = lookAt.position + offset_interpolate;
            rot_transition += Time.deltaTime * 1 / rotationDuration;
        }

        if (rot_transition > 1.0f)
        {
            moveVector = lookAt.position + cur_offset;
            last_offset = cur_offset;
            rot_transition += Time.deltaTime * 1 / rotationDuration;
        }


        //X
        //moveVector.x = 0;
        //Y
        //moveVector.y = Mathf.Clamp(moveVector.y, 3, 5);
        //Z
        if (transition > 1.0f)
        {
            //float dx, dy, dz;
            float sx, sy, sz;
            dz = 0.0f;
            sx = 0.0f;
            sy = 0.0f;
            sz = 0.0f;
            if (fr.w != 0)
            {
                face_cx = fr.x + fr.w * 0.5f;
                face_cy = fr.y + fr.h * 0.5f;

                dx = (cam_width * 0.5f - face_cx) * pers_ratio.x;
                dy = (cam_height * 0.5f - face_cy) * pers_ratio.y;
                dz = (fr.w - face_width) * pers_ratio.z;

                theta = dx * Mathf.PI * 0.5f / (cam_width * 0.2f * pers_ratio.x);
                phi = dy * Mathf.PI * 0.5f / (cam_height * 0.2f * pers_ratio.y);


                sx = rho * Mathf.Cos(phi) * Mathf.Sin(theta);
                sy = rho * Mathf.Sin(phi);
                sz = -rho * Mathf.Cos(phi) * Mathf.Cos(theta) + dz * 2.0f;
            }
            //cam_pos.x = moveVector.x + dx;
            //cam_pos.y = moveVector.y + dy;
            //cam_pos.z = moveVector.z + dz;




            //cam_pos = moveVector + Quaternion.Euler(lookAt.eulerAngles) * (new Vector3(dx, dy, dz));
            cam_pos = moveVector + Quaternion.Euler(lookAt.eulerAngles) * Quaternion.AngleAxis(start_angle, Vector3.right) * (new Vector3(sx, sy, sz));

            transform.position = cam_pos;
            transform.LookAt(lookAt.position + Vector3.up);
        }
        else
        {
            if (fr.w != 0)
            {
                face_width = fr.w;
            }
            if (fr.h != 0)
            {
                face_height = fr.h;
            }

            //Animation at the start of the game
            transform.position = Vector3.Lerp(moveVector + animationOffset, moveVector, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            transform.LookAt(lookAt.position + Vector3.up);
        }

        //transform.LookAt(lookAt.position+Vector3.up);
        //transform.position = moveVector;
    }
    void OnApplicationQuit()
    {
        close_capture();
    }
    private void get_face_pos()
    {
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
                    face_cx = fr.x + fr.w * 0.5f;
                    face_cy = fr.y + fr.h * 0.5f;
                    //double newx = (cam_width * 0.5 - cx) * pers_ratio.x + cam_pos.x;
                    //double newy = (cam_height * 0.5 - cy) * pers_ratio.y + cam_pos.y;
                    //this.transform.position = new Vector3((float)newx, (float)newy, cam_pos.z);
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
