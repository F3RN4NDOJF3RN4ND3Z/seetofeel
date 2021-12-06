using StereoKit;
using StereoKit.Framework;
using System;

namespace SeeToFeelXP
{
    public class App
    {

        static Pose windowPoseHistory = new Pose(0.4f, 0.5f, -0.4f, Quat.LookDir(-1, 0, 1));
        static Pose windowPoseMeassure = new Pose(-0.4f, 0f, -0.4f, Quat.LookDir(1, 0, 1));
        static Pose windowPoseHelp = new Pose(0f, 0.2f, -0.5f, Quat.LookDir(0, 0, 1));
        static Pose windowPoseVideo = new Pose(0f, 0.2f, -0.5f, Quat.LookDir(0, 0, 1));
        static Random rnd = new Random();
        Model table;
        Model med_kit ;
        Model syrenge;
        Model vial;
        Matrix floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
        Material floorMaterial;
        Pose tablePose = new Pose(0, -1.5f, -2, Quat.Identity);
        Pose medkitPose = new Pose(0.5f, -0.25f, -0.6f, Quat.Identity);
        Pose syrengePose = new Pose(0f, -0.22f, -0.4f, Quat.FromAngles(0, 90, 0));
        Pose vialPose = new Pose(-0.2f, -0.25f, -0.4f, Quat.Identity);
        bool displayHistory = false, displayMeasure = false, displayHelp = true, displayPlayer=false;
        int meassureGlucose = 0;
        float timeelapsed = 0;
        Anim syrengeFill;

        public SKSettings Settings => new SKSettings
        {
            appName = "SeeToFeel",
            assetsFolder = "Assets",
            displayPreference = DisplayMode.MixedReality
        };

        public void Init()
        {



            //Setup assets used by the app
            floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
            floorMaterial.Transparency = Transparency.Blend;

            table = Model.FromFile("table.obj");
            med_kit = Model.FromFile("medical_kit.gltf");
            syrenge = Model.FromFile("syrenge.glb");
            vial = Model.FromFile("vial.gltf");
            //Mesh testcube = Mesh.GenerateCube(Vec3.One * 0.1f,0);
            //Pose cubePose = new Pose(0, 0.2f, -0.5f, Quat.Identity);

            //Mesh testcube2 = Mesh.GenerateCube(Vec3.One * 0.1f, 0);
            //Pose cubePose2 = new Pose(0.5f, 0.2f, -0.5f, Quat.Identity);

            syrenge.AddNode("Needle", Matrix.TS(syrenge.Bounds.center - new Vec3 (0,0,-0.08f), 0.01f), null, null);
            
            syrengeFill =syrenge.FindAnim("pistonAction.002");
            syrenge.PlayAnim(syrengeFill, AnimMode.Manual);

            //Create Hand Menu
            HandMenuRadial handMenu = SK.AddStepper(new HandMenuRadial(
            new HandRadialLayer("Glucometer",
                new HandMenuItem("Meassure", null, null, "Meassure"),
                new HandMenuItem("History", null, () => displayHistory = true),
                new HandMenuItem("Help", null, () => displayHelp = true)),
            new HandRadialLayer("Meassure",
                new HandMenuItem("New", null, () => {
                    displayMeasure = true;
                    meassureGlucose = rnd.Next(50, 150);
                }),
                new HandMenuItem("Back", null, null, HandMenuAction.Back))
           ));

            //HandJoint[] joints = new HandJoint[] { new HandJoint(new Vec3(0, 0, 0), new Quat(-0.445f, -0.392f, 0.653f, -0.472f), 0.021f), new HandJoint(new Vec3(0, 0, 0), new Quat(-0.445f, -0.392f, 0.653f, -0.472f), 0.021f), new HandJoint(new Vec3(0, 0, 0), new Quat(-0.342f, -0.449f, 0.618f, -0.548f), 0.014f), new HandJoint(new Vec3(0.139f, -0.151f, -0.193f), new Quat(-0.409f, -0.437f, 0.626f, -0.499f), 0.010f), new HandJoint(new Vec3(0.141f, -0.133f, -0.198f), new Quat(-0.409f, -0.437f, 0.626f, -0.499f), 0.010f), new HandJoint(new Vec3(0.124f, -0.229f, -0.172f), new Quat(0.135f, -0.428f, 0.885f, -0.125f), 0.024f), new HandJoint(new Vec3(0.103f, -0.184f, -0.209f), new Quat(0.176f, -0.530f, 0.774f, -0.299f), 0.013f), new HandJoint(new Vec3(0.078f, -0.153f, -0.225f), new Quat(0.173f, -0.645f, 0.658f, -0.349f), 0.010f), new HandJoint(new Vec3(0.061f, -0.135f, -0.228f), new Quat(-0.277f, 0.674f, -0.623f, 0.283f), 0.010f), new HandJoint(new Vec3(0.050f, -0.125f, -0.227f), new Quat(-0.277f, 0.674f, -0.623f, 0.283f), 0.010f), new HandJoint(new Vec3(0.119f, -0.235f, -0.172f), new Quat(0.147f, -0.399f, 0.847f, -0.318f), 0.024f), new HandJoint(new Vec3(0.088f, -0.200f, -0.211f), new Quat(0.282f, -0.603f, 0.697f, -0.268f), 0.012f), new HandJoint(new Vec3(0.056f, -0.169f, -0.216f), new Quat(-0.370f, 0.871f, -0.308f, 0.099f), 0.010f), new HandJoint(new Vec3(0.045f, -0.156f, -0.195f), new Quat(-0.463f, 0.884f, -0.022f, -0.066f), 0.010f), new HandJoint(new Vec3(0.047f, -0.155f, -0.178f), new Quat(-0.463f, 0.884f, -0.022f, -0.066f), 0.010f), new HandJoint(new Vec3(0.111f, -0.244f, -0.173f), new Quat(0.182f, -0.436f, 0.778f, -0.414f), 0.022f), new HandJoint(new Vec3(0.074f, -0.213f, -0.205f), new Quat(-0.353f, 0.622f, -0.656f, 0.244f), 0.011f), new HandJoint(new Vec3(0.046f, -0.189f, -0.204f), new Quat(-0.436f, 0.891f, -0.073f, -0.108f), 0.010f), new HandJoint(new Vec3(0.048f, -0.184f, -0.182f), new Quat(-0.451f, 0.811f, 0.264f, -0.263f), 0.010f), new HandJoint(new Vec3(0.061f, -0.188f, -0.168f), new Quat(-0.451f, 0.811f, 0.264f, -0.263f), 0.010f), new HandJoint(new Vec3(0.105f, -0.250f, -0.170f), new Quat(0.219f, -0.470f, 0.678f, -0.521f), 0.020f), new HandJoint(new Vec3(0.062f, -0.228f, -0.196f), new Quat(-0.444f, 0.610f, -0.623f, 0.206f), 0.010f), new HandJoint(new Vec3(0.044f, -0.215f, -0.192f), new Quat(-0.501f, 0.841f, -0.094f, -0.183f), 0.010f), new HandJoint(new Vec3(0.048f, -0.209f, -0.176f), new Quat(-0.521f, 0.682f, 0.251f, -0.448f), 0.010f), new HandJoint(new Vec3(0.061f, -0.207f, -0.168f), new Quat(-0.521f, 0.682f, 0.251f, -0.448f), 0.010f), new HandJoint(new Vec3(0.098f, -0.222f, -0.191f), new Quat(0.308f, -0.906f, 0.288f, -0.042f), 0.000f), new HandJoint(new Vec3(0.131f, -0.251f, -0.164f), new Quat(0.188f, -0.436f, 0.844f, -0.248f), 0.000f) };

     
        }
        public void Step()
        {
            if (SK.System.displayType == Display.Opaque)
                Default.MeshCube.Draw(floorMaterial, floorTransform);

            UI.Handle("Syrenge", ref syrengePose, syrenge.Bounds);


            UI.Handle("Vial", ref vialPose, vial.Bounds);
            //Input.HandOverride(Handed.Right, joints);



            //UI.Handle("cube", ref cubePose, testcube.Bounds);

            //UI.Handle("cube2", ref cubePose2, testcube2.Bounds);

            table.Draw(tablePose.ToMatrix(0.017f));
            syrenge.Draw(syrengePose.ToMatrix());
            vial.Draw(vialPose.ToMatrix());
            med_kit.Draw(medkitPose.ToMatrix());

            //testcube.Draw(Default.MaterialUI, cubePose.ToMatrix());
            //testcube2.Draw(Default.MaterialUI, cubePose2.ToMatrix());

            Matrix vialPoseTransform = Matrix.TS(vial.Bounds.center, vial.Bounds.dimensions) * vialPose.ToMatrix();
            Matrix syrengePoseTransform = Matrix.TS(syrenge.Bounds.center, syrenge.Bounds.dimensions) * syrengePose.ToMatrix();

            //Mesh.Cube.Draw(Material.UIBox, Matrix.TS(vial.Bounds.center, vial.Bounds.dimensions) * vialPose.ToMatrix());
            //Mesh.Cube.Draw(Material.UIBox, Matrix.TS(syrenge.Bounds.center, syrenge.Bounds.dimensions) * syrengePose.ToMatrix());

            Matrix needlePoseTransform = Matrix.TS(syrenge.Bounds.center - new Vec3(0, 0, -0.08f), 0.01f) * syrengePose.ToMatrix();

            Bounds syrengeBounds = new Bounds(syrengePoseTransform.Pose.position, syrenge.Bounds.dimensions);
            Bounds vialBounds = new Bounds(vialPoseTransform.Pose.position, vial.Bounds.dimensions);
            Bounds needleBound= new Bounds(needlePoseTransform.Pose.position, syrenge.Bounds.dimensions);

            Pose fingertip = Input.Hand(Handed.Left)[FingerId.Index, JointId.Tip].Pose;

            //Lines.Add(vialPoseTransform.Pose.position, syrengePoseTransform.Pose.position, Color.White, 0.01f);
            //Lines.AddAxis(cubePose,0.1f);
            //Lines.AddAxis(cubePose2, 0.1f);
            //Lines.Add(cubePose.position, cubePose2.position, Color.White, 0.01f);
            //Lines.AddAxis(vialPoseTransform.Pose, 0.2f);
            //Lines.AddAxis(syrengePoseTransform.Pose, 0.2f);
            //Lines.AddAxis(needlePoseTransform.Pose, 0.2f);


            Vec3 intersectionPoint;
            Ray syringeRay = vialPoseTransform.Transform(syrengePoseTransform.Pose.Ray);

            //bool inVial = syringeRay.Intersect(vial.Bounds, out intersectionPoint);
            bool inVial = vialBounds.Contains(needlePoseTransform.Pose.position);
            bool inHand = needleBound.Contains(fingertip.position);
            //Log.Info("anim completion:" + syrenge.AnimCompletion);
            if (inVial)
            {
              
                if (syrenge.AnimTime < syrengeFill.Duration){
                    syrenge.AnimTime += 0.01f;
                }
                
            }

            if (inHand)
            {
                if (syrenge.AnimTime > 0)
                {
                    syrenge.AnimTime -= 0.01f;
                }
            }


            ShowHistoryWindow(ref displayHistory);

            ShowMeassureWindow(ref displayMeasure, ref meassureGlucose);

            ShowHelpWindow(ref displayHelp);

            ShowVideoPlayer(ref displayPlayer, ref timeelapsed);
        }

        static void ShowHistoryWindow(ref bool display)
        {
            if (display)
            {
                UI.WindowBegin("History of Glocosamine", ref windowPoseHistory);

                UI.Image(Sprite.FromFile("histogram.png"), new Vec2(0.5f, 0.3f));
                if (UI.Button("Close")) {
                    display = false;
                }
                

                UI.WindowEnd();
            }
            
        }

        static void ShowVideoPlayer(ref bool display, ref float timeelapsed)
        {
            if (display)
            {
                UI.WindowBegin("Video Player", ref windowPoseVideo);
                
                UI.Image(Sprite.FromFile("histogram.png"), new Vec2(0.5f, 0.3f));
                if (UI.Button("Play"))
                {
                    //display = false;
                }
                if (UI.Button("Pause"))
                {
                   //display = false;
                }
                if (UI.Button("Close"))
                {
                    //display = false;
                }


                UI.WindowEnd();
            }

        }

        static void ShowMeassureWindow(ref bool display, ref int meassure)
        {
            if (display)
            {
             
                
                UI.WindowBegin("Meassure Glocosamine", ref windowPoseMeassure);
                UI.Text(meassure + "mg/PL", TextAlign.Center);
                if (UI.Button("Close!"))
                {
                    display = false;
                    meassure = 0;
                }
               

                UI.WindowEnd();
            }

        }


        static void ShowHelpWindow(ref bool display)
        {
            if (display)
            {
                UI.WindowBegin("About", ref windowPoseHelp,new Vec2(0.5f,0.2f),UIWin.Normal,UIMove.FaceUser);
                UI.Text("Welcome to See to Feel",TextAlign.Center);
                UI.Text("Welcome to this Educational health care experience.");
                UI.Text("Here you can come to understand how a patient with Diabetes take care of himself.");
                UI.Text("You can grab de syrenge and the vial");
                UI.Text("You can also use the menu in your right hand. Just Turn it facing the palm to you and make a fist. You can meeasure your Glucosamine from there");
                UI.Text("Also view some graphs of your meassurements");
                UI.Text("\n\nCreated By 3GOVideo www.3govideo.com");

                if (UI.Button("Close"))
                {
                    display = false;
                }


                UI.WindowEnd();
            }

        }
    }
}
