using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Xml.Serialization;
using ImpMove.Core;
using ImpMove.GUI;
using ImpMove.Helpers;
using Styx;
using Styx.Common;
using Styx.Common.Helpers;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Plugins;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace ImpMove
{
    public class ImpMovePlugin:HBPlugin
    {
        #region Standart

        public override string Name
        {
            get { return "ImpMovePlugin"; }
        }

        public override string Author
        {
            get { return "DKBot"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        #endregion

        public Composite Root { get; set; }
        private static LocalPlayer Me { get { return StyxWoW.Me; } }
        public static string SettingsPath
        {
            get
            {
                var settingsDirectory = Path.Combine(Utilities.AssemblyDirectory, "Plugins\\ImpMovePlugin");
                return Path.Combine(Path.Combine(settingsDirectory, StyxWoW.Me.Name+".xml"));
            }
        }

        public static bool NeedMove { get; set; }
        public static WoWPoint PointTo { get; set; }

        public static WoWPoint CalculatePoint
        {
            get { return WoWMathHelper.CalculatePointFrom(Me.Location, PointTo, 5); }
        }

        public static List<WoWPoint> PathNav { get; set; }
        public static bool Los { get; set; }
        public static List<ImpPoint> MyPointsList { get; set; } 

        private static readonly WaitTimer WaitNextMessage = WaitTimer.ThirtySeconds;
        public static int PerSecond;

        public override void OnEnable()
        {
            base.OnEnable();
            PerSecond = 1;
            Root = CreateBehaviorLogic();
            Root.Start(null);
            BotEvents.OnBotStarted+=OnStart;
            BotEvents.OnBotStopped += OnStop;
            NeedMove = false;
            PointTo = WoWPoint.Empty;
            Los = false;
            MyPointsList=new List<ImpPoint>();
            PathNav=new List<WoWPoint>();
        }

        private static void OnStop(EventArgs args)
        {
            Logs.Log("Стоп");
            AlertWisper();
        }

        private static void OnStart(EventArgs args)
        {
          Logs.Log("Начали");
            AlertWisper();
            

        }

        public override void OnDisable()
        {
            BotEvents.OnBotStarted -= OnStart;
            BotEvents.OnBotStopped -= OnStop;
            Root.Stop(null);
            Root = null;
            base.OnDisable();
        }

        public override bool WantButton
        {
            get { return true; }
        }

        public override void OnButtonPress()
        {
            var gui = new Form1();
            gui.Show();
        }

        public override void Pulse()
        {
            if(!StyxWoW.IsInGame||!StyxWoW.IsInWorld||!Me.IsValid)return;
         //   try
          //  {
               // if (DateTime.Now.Ticks>LastTick)
               // {
                    Root.Tick(null);
                    // If the last status wasn't running, stop the tree, and restart it.
                    if (Root.LastStatus != RunStatus.Running)
                    {
                        Root.Stop(null);
                        Root.Start(null);
                    }
               //     LastTick = DateTime.Now.Ticks + (TimeSpan.TicksPerSecond * PerSecond);
              //  }
               
               
         //   }
            //catch (Exception e)
            //{
            //    // Restart on any exception.
            //    Logging.WriteException(e);
            //    Root.Stop(null);
            //    Root.Start(null);
            //    throw;
            //}
        }

/*
        private long LastTick { get; set; }
*/


        private static Composite CreateBehaviorLogic()
        {
            return new PrioritySelector(
                 Move.NeedMove(),
               // Move.CompositeJumpMeele(),
                // .. logic for behavior tree goes here ..

                new Action(r =>
                {
                    if (WaitNextMessage.IsFinished)
                    {
                        WaitNextMessage.Reset();
                        var obj =
                            ObjectManager.GetObjectsOfTypeFast<WoWGameObject>()
                                .Where(ob => ob.IsValid )
                                .OrderBy(od => od.Distance)
                                .FirstOrDefault();
                        var unit =
                            ObjectManager.GetObjectsOfTypeFast<WoWPlayer>()
                                .Where(player => player.IsValid && player.IsAlliance)
                                .OrderBy(p => p.Distance)
                                .FirstOrDefault();
                        if (obj != null)
                        {

                            var list = obj.GetType().GetProperties().Select(w => w.Name + " - " + w.GetValue(obj));
                            var list3 = list.ToList();
                            var list2 = obj.GetType().GetFields().Select(w => w.Name + " - " + w.GetValue(obj));
                            list3.AddRange(list2);
                            var file = list3.Find(p => p.Contains("Entry")) ?? list3.Find(p => p.Contains("DescriptorGuid "));
                            file = file.Remove(0, file.LastIndexOf(' ') + 1);
                            if (File.Exists("BASE\\" + file + ".xml"))return;
                            SaveIdList("BASE\\" + file + ".xml", list3.ToList());
                            Logging.Write("Добавили {0} {1}",obj.Name,obj.Entry);

                        }
                        if (unit != null)

                        {
                            AlertWisper();
                            var list = unit.GetType().GetProperties().Select(w => w.Name + " - " + w.GetValue(unit));
                            var list3 = list.ToList();
                            var list2 = unit.GetType().GetFields().Select(w => w.Name + " - " + w.GetValue(unit));
                            list3.AddRange(list2);
                            var file = list3.Find(p => p.Contains("Name")) ?? list3.Find(p => p.Contains("SafeName"));
                            file = file.Remove(0, file.LastIndexOf(' ') + 1);
                            if (File.Exists("BASE\\" + file + ".xml")) return;
                            SaveIdList("BASE\\" + file + ".xml", list3.ToList());
                            Logging.Write("Добавили {0} {1}", unit.Name, unit.SafeName);

                        }
                        //Logging.Write(Colors.White, "[ImpMove] activity message " + DateTime.Now);
                       
                    }
                })
                );
        }

        private static void AlertWisper()
        {
           
            new SoundPlayer(SoundFileWhisper).Play();
          
        }

        private static string SoundFileWhisper
        {
            get
            {
                var varSPath = Path.Combine(FolderPath, @"beep.wav");
                return varSPath;
            }
        }
        public static string FolderPath
        {
            get
            {
                var sPath = Path.GetFullPath(Application.StartupPath + "\\Plugins\\ImpMove\\");
                return sPath;
            }
        }

        private static void SaveIdList(string filein, List<string> listin)
        {
            var file = filein;
            var list = listin;


            using (var f = File.CreateText(file))
            {
                var writer = new XmlSerializer(list.GetType());

                writer.Serialize(f, list);
            }
        }
    }
}
