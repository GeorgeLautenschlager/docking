using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        bool setupcomplete = false;
        bool requestSent = false;
        bool clearanceReceived = false;
        IMyRadioAntenna antenna;
        IMyProgrammableBlock pb;
        MyIGCMessage clearance = new MyIGCMessage();
        string tag1 = "Traffic Control";

        public void Main(string arg)
        {
            if (setupcomplete == false)
            {
                Echo("Running setup.");
                Setup();
            }
            else if(setupcomplete == true && requestSent == false)
            {
                string messageOut = "Requesting Docking Clearance";
                IGC.SendBroadcastMessage(tag1, messageOut, TransmissionDistance.TransmissionDistanceMax);
                Echo("Requesting Clearance");
                requestSent = true;
            }
            else if(requestSent == true && clearanceReceived == false)
            {
                // Now that we've requested clearance, listen for a clearance response.
                IGC.RegisterBroadcastListener(tag1);
                List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
                IGC.GetBroadcastListeners(listeners);
                Echo("checking messages");
                if (listeners[0].HasPendingMessage)
                {
                    Echo("message received");
                    clearance = listeners[0].AcceptMessage();
                    clearanceReceived = true;
                }
                else
                {
                    requestSent = false;
                }
            }
            else if(clearanceReceived == true)
            {
                Echo(clearance.Data.ToString());
            }
        }

        public void Setup()
        {
            antenna = GridTerminalSystem.GetBlockWithName("Antenna") as IMyRadioAntenna;
            pb = GridTerminalSystem.GetBlockWithName("Programmable block") as IMyProgrammableBlock;

            if (antenna != null)
            {
                Echo("Setup complete.");
                setupcomplete = true;
            }
            else
            {
                Echo("Setup failed. No antenna found.");
            }
        }
    }
}
