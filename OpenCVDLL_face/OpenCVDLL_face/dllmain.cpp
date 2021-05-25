// dllmain.cpp : DLL 애플리케이션의 진입점을 정의합니다.
/*---
https://github.com/Anim4bot/HeadTracking/blob/master/Head_Tracking%20-%20V1.py
https://docs.opencv.org/3.4/db/d28/tutorial_cascade_classifier.html
http://thomasmountainborn.com/2017/03/05/unity-and-opencv-part-three-passing-detection-data-to-unity/
---*/
#include "pch.h"
#include <opencv2/core.hpp>
#include <opencv2/videoio.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/objdetect.hpp>
//#include <math.h>

#ifndef M_PI 
#define M_PI 3.1415926535
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

using namespace cv;
using namespace std;


CascadeClassifier face_cascade;
VideoCapture cap;

struct FaceRect
{
	FaceRect(int x, int y, int w, int h) : x(x), y(y), w(w), h(h) {}
	int x, y, w, h;
};
struct Color32
{
	uchar red;
	uchar green;
	uchar blue;
	uchar alpha;
};


void rotate_image(Mat& src, Mat& dst, int width, int height, float angle) {
	if (angle == 0.0) {
		dst = src.clone();
		return;
	}
	Mat rot_mat = getRotationMatrix2D(Point2f(width / 2.0, height / 2.0), angle, 1.0);
	warpAffine(src, dst, rot_mat, Size(width, height), INTER_LINEAR);
}
void rotate_rect(FaceRect & fr_src, FaceRect & fr_dst, int width, int height, float angle) {
	if (angle == 0.0) {
		fr_dst = FaceRect(fr_src);
		return;
	}
	int cx = fr_src.x + fr_src.w * 0.5 - width * 0.5;
	int cy = fr_src.y + fr_src.h * 0.5 - height * 0.5;
	int newcx = cx * cos(angle * M_PI / 180.0) - cy * sin(angle * M_PI / 180.0) + width * 0.5;
	int newcy = cx * sin(angle * M_PI / 180.0) + cy * cos(angle * M_PI / 180.0) + height * 0.5;
	fr_dst = FaceRect(newcx - fr_src.w * 0.5, newcy - fr_src.h * 0.5, fr_src.w, fr_src.h);

}
extern "C" {
	void __declspec(dllexport)  FlipImage(Color32** rawImage, int width, int height)
	{
		using namespace cv;

		Mat image(height, width, CV_8UC4, *rawImage);

		flip(image, image, 1);
	}
	//--- INITIALIZE VIDEOCAPTURE

	int __declspec(dllexport) init_capture(const char* face_cascade_file, int& cam_width, int& cam_height, int& cam_fps)
	{
		//String face_cascade_file = getenv("OPENCV_DIR") + String("\\data\\haarcascades\\haarcascade_frontalface_alt.xml");
		//String face_cascade_file = getenv("OPENCV_DIR") + String("\\data\\haarcascades\\haarcascade_eye_tree_eyeglasses.xml");

		// open the default camera using default API
		// cap.open(0);
		// OR advance usage: select any API backend
		int deviceID = 0;             // 0 = open default camera
		int apiID = CAP_ANY;      // 0 = autodetect default API

		if (!face_cascade.load(face_cascade_file))
		{
			//cout << "--(!)Error loading face cascade\n";
			return -1;
		}
		// open selected camera using selected API
		cap.open(deviceID, apiID);
		// check if we succeeded
		if (!cap.isOpened()) {
			//cerr << "ERROR! Unable to open camera\n";
			return -1;
		}
		cam_width = cap.get(CAP_PROP_FRAME_WIDTH);
		cam_height = cap.get(CAP_PROP_FRAME_HEIGHT);
		cam_fps = cap.get(CAP_PROP_FPS);
		return 0;
	}
	void __declspec(dllexport) get_frame_pos(int& cam_frame_pos) {
		cam_frame_pos= cap.get(CAP_PROP_POS_FRAMES);
	}
	void __declspec(dllexport)  detect_rect(FaceRect& fr, float* rot_angles, int rot_len, int width, int height, int camVis) {
		Mat frame, frame_gray, frame_resized, frame_rot;
		vector<Rect> face;
		int HeadMinSize_val = 30;
		int HeadMaxSize_val = 600;
		int minNeighbors_val = 5;
		float scaleFactor_val = 1.10;
		Scalar head_color = Scalar(255, 255, 255);


		cap.read(frame);

		// check if we succeeded
		if (frame.empty()) {
			//cerr << "ERROR! blank frame grabbed\n";
			return;
		}
		resize(frame, frame_resized, Size(width, height));
		cvtColor(frame_resized, frame_gray, COLOR_BGR2GRAY);
		for (int i = 0; i < rot_len; i++) {
			float angle = rot_angles[i];
			rotate_image(frame_gray, frame_rot, width, height, angle);
			face_cascade.detectMultiScale(frame_rot, face, scaleFactor_val, minNeighbors_val, 2,
				Size(HeadMinSize_val, HeadMinSize_val),
				Size(HeadMaxSize_val, HeadMaxSize_val));
			if (face.size() > 0) {
				for (int fi = 0; fi < 1; fi++) {
					fr = FaceRect(face[fi].x, face[fi].y, face[fi].width, face[fi].height);
					FaceRect fr_rot = FaceRect(fr);
					//circle(frame_rot, Point(fr.x + fr.w * 0.5, fr.y + fr.w * 0.5), 2, head_color, 2);
					//rectangle(frame_rot, Rect(fr.x, fr.y, fr.w, fr.h), head_color, 2);
					rotate_rect(fr, fr_rot, width, height, angle);
					circle(frame_gray, Point(fr_rot.x + fr.w * 0.5, fr_rot.y + fr.w * 0.5), 2, head_color, 2);
					rectangle(frame_gray, Rect(fr_rot.x, fr_rot.y, fr_rot.w, fr_rot.h), head_color, 2);
				}
				break;
			}
		}

		if (camVis) {
			// show live and wait for a key with timeout long enough to show images
			//imshow("Live_rotation", frame_rot);
			flip(frame_gray, frame_gray, 1);
			imshow("Live_gray", frame_gray);
		}
	}
	void __declspec(dllexport) close_capture()
	{
		cap.release();
	}
}
