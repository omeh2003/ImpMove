using System;
using System.Linq;
using CommonBehaviors.Actions;
using Styx;
using Styx.Common;
using Styx.CommonBot.Coroutines;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Action = Styx.TreeSharp.Action;

namespace ImpMove.Core
{
    static class Move
    {
        private static Composite MoveTo()
        {
            return new PrioritySelector(
                new DecoratorContinue(ret => Navigator.CanNavigateWithin(StyxWoW.Me.Location, ImpMovePlugin.PointTo,5) && ImpMovePlugin.PointTo.Distance(StyxWoW.Me.Location)>=5,
                    new Action(ret=>CommonCoroutines.MoveTo(ImpMovePlugin.CalculatePoint))),
                    new Decorator(ret => ImpMovePlugin.PointTo.Distance(StyxWoW.Me.Location) <5,
                        new Action(ret =>
                        {
                            if (ImpMovePlugin.PathNav.Any())
                            {
                                Logging.Write("PathNav не пустой; Старая точка {0}", ImpMovePlugin.PointTo);
                                var a=ImpMovePlugin.PathNav.Remove(ImpMovePlugin.PointTo);
                                Logging.Write("Удалил старую точку из пути - {0}",a);
                              

                            }
                            if (ImpMovePlugin.PathNav.Any())
                            {
                                ImpMovePlugin.PointTo = ImpMovePlugin.PathNav.FirstOrDefault();
                                Logging.Write("Новая точка - {0}", ImpMovePlugin.PointTo);
                                return;

                            }
                            if (!ImpMovePlugin.PathNav.Any()&&ImpMovePlugin.PointTo!=WoWPoint.Empty)
                            {
                                Navigator.Clear();
                                Logging.Write("Сбросил навигатор");
                                ImpMovePlugin.NeedMove = false;
                                Logging.Write("Need Move - false");
                                ImpMovePlugin.PointTo = WoWPoint.Empty;
                                Logging.Write("PointTo - Empty");
                                CountGenerate = 0;
                            }
                        })
                        ),

                        new Decorator(ret => !Navigator.CanNavigateWithin(StyxWoW.Me.Location, ImpMovePlugin.PointTo, 5) && ImpMovePlugin.NeedMove,
                        new Action(ret =>
                        {
                            CountGenerate++;
                            Logging.Write("Пробую сделать путь {0}",CountGenerate);
                           
                            //LoadMesh();
                            if (CountGenerate >= 10)
                            {
                                Navigator.Clear();
                                Logging.Write("Сбросил навигатор Попыток больше 10");
                                ImpMovePlugin.PointTo = WoWPoint.Empty;
                                ImpMovePlugin.NeedMove = false;
                                CountGenerate = 0;
                                return;
                            }
                            var location = StyxWoW.Me.Location;
                            var path = Navigator.GeneratePath(location, ImpMovePlugin.PointTo).ToList();
                            if (path.Any() && !ImpMovePlugin.PathNav.Any())
                            {
                                Logging.Write("найден путь. в нем {0} элементов",path.Count);
                                
                                ImpMovePlugin.PathNav =
                                    path.ToList();
                            }
                           if(ImpMovePlugin.PathNav.Any())
                           {
                               
                               ImpMovePlugin.PointTo = ImpMovePlugin.PathNav.FirstOrDefault();
                               Logging.Write("PointTo = PathNav.FirstOrDefault. "+ImpMovePlugin.PointTo);
                           }
                         


                        })
                        )
                    
               
                );
        }

        public static int CountGenerate { get; set; }

        public static Composite NeedMove()
        {
            return new PrioritySelector(
               

                new DecoratorContinue(ret => ImpMovePlugin.NeedMove&&ImpMovePlugin.PointTo!=WoWPoint.Empty,
                   MoveTo()),
                   new DecoratorContinue(ret => !ImpMovePlugin.NeedMove&&ImpMovePlugin.Los,
                   CompositeJumpMeele())
                   
                );

        }


        private static bool LastRight { get; set; }


        public static Composite CompositeJumpMeele()
        {
            return
                new PrioritySelector(
                    new DecoratorContinue(ret => ImpMovePlugin.Los,
                        new Sequence(
                            new Action(ret => WoWMovement.Move(WoWMovement.MovementDirection.Forward)),
                            new Action(ret => WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend)),
                            new Action(
                                ret => StyxWoW.Me.SetFacing((float)((Math.PI + StyxWoW.Me.Rotation) % (2 * Math.PI)))),
                            new WaitContinue(TimeSpan.FromMilliseconds(1000), ret => false, new ActionAlwaysSucceed()),
                            new Action(ret => WoWMovement.Move(WoWMovement.MovementDirection.Forward)),
                            new Action(ret => WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend)),
                            new WaitContinue(TimeSpan.FromMilliseconds(750), ret => false, new ActionAlwaysSucceed()),
                            new Action(
                                ret =>
                                    WoWMovement.Move(LastRight
                                        ? WoWMovement.MovementDirection.TurnLeft
                                        : WoWMovement.MovementDirection.TurnRight)),
                            new Action(ret => LastRight = !LastRight),


                            new WaitContinue(TimeSpan.FromMilliseconds(1300), ret => false, new ActionAlwaysSucceed()),
                            new Action(ret => WoWMovement.MoveStop())
                            )
                        )
                    );
        }


/*
        private static void LoadMesh()
        {
            Logging.Write("Загрузка тилей начинается");
            var point = StyxWoW.Me.Location;
            var nav=new Tripper.Navigation.WowNavigator();

            var tile = Tripper.MeshMisc.TileIdentifier.GetByPosition(point.X, point.Y);

            if (nav.LoadTile(tile)) { Logging.Write("Ок");}
                nav.OnTileLoaded += nav_OnTileLoaded;
            

        }
*/

/*
        static void nav_OnTileLoaded(object sender, TileLoadedEventArgs e)
        {
          Logging.Write("Загрузка тилей заверщена");
        }
*/

    }
}
