using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebusSQL6
{
    public class RebusNew
    {
        public class Things
        {
            private List<Thing> moThings;
            public List<Thing> Thingies { get { return moThings; } }


            //
            // constructor
            //
            public Things()
            {
                InitializeThings();       // we'll start "things" as not null
            }

            private void InitializeThings()
            {
                moThings = new List<Thing>(0);
            }

            public void GetThingsFromFile(string psFile)
            {
                StreamReader xoRdr = null;
                Thing xoCurrThing = null;
                //int xiThings = 0;

                InitializeThings();

                try
                {
                    xoRdr = new StreamReader(psFile);
                    while (!xoRdr.EndOfStream)
                    {
                        string xsLine = xoRdr.ReadLine();
                        xsLine = xsLine.TrimEnd();
                        string xs = xsLine.Replace("\t", "");
                        if (xs.ToUpper() == "NEW")
                        {
                            if (xoCurrThing != null)
                            {
                                moThings.Add(xoCurrThing);
                                xoCurrThing = new Thing();
                            }
                            else
                            {
                                xoCurrThing = new Thing();
                            }
                        }
                        else
                        {
                            if (xoCurrThing != null)
                            {
                                int xi = xsLine.IndexOf("=");
                                if (xi > 0)
                                {
                                    string xsProp = xsLine.Substring(0, xi);
                                    string xsVal = "";
                                    if (xsLine.Length - 1 > xi) xsVal = xsLine.Substring(xi + 1);
                                    try
                                    {
                                        xoCurrThing.AddProperty(xsProp.Trim(), xsVal.Trim());
                                    }
                                    catch {  }
                                }
                            }
                        }
                    }
                }
                catch (Exception xoExc)
                {
                    throw new ThingException(xoExc.Message);
                }

                if (xoRdr != null)
                {
                    try
                    {
                        xoRdr.Close();
                    }
                    catch { }
                    xoRdr.Dispose();
                    xoRdr = null;
                }

                if (xoCurrThing != null) moThings.Add(xoCurrThing);
                xoCurrThing = null;
            }


        }           // end class Things


        public class Thing
        {
            private Dictionary<string, string> moThing;
            public Dictionary<string, string> Thingy { get { return moThing; } }



            //
            // constructor
            //
            public Thing()
            {
                moThing = new Dictionary<string, string>(0);       // we'll start a "thing" as not null
            }

            public void AddProperty(string psProperty, string psValue)
            {
                string xsProp = psProperty.Trim().ToUpper();

                if (moThing.ContainsKey(xsProp))
                {
                    throw new ThingException(psProperty + " already exists.");
                }
                else
                {
                    moThing.Add(xsProp, psValue.Trim());
                }
            }

            public string GetPropertyValue(string psProperty)
            {
                string xsProp = psProperty.Trim().ToUpper();
                string xsValue = "";

                try
                {
                    xsValue = moThing[xsProp];
                }
                catch
                {
                    throw new ThingException(psProperty + " not found.");
                }

                return (xsValue);
            }
            public void ChangePropertyValueTo(string psProperty, string psValue)
            {
                string xsProp = psProperty.Trim().ToUpper();

                try
                {
                    moThing[xsProp] = psValue;
                }
                catch
                {
                    throw new ThingException(psProperty + " not found.");
                }
            }

            public void RemoveProperty(string psProperty)
            {
                string xsProp = psProperty.Trim().ToUpper();
                bool xbErr = false;
                
                try
                {
                    moThing.Remove(xsProp);
                }
                catch
                {
                    xbErr = true;
                }

                if (xbErr) throw new ThingException(psProperty + " not found.");
            }

            public void DeleteProperty(string psProperty)
            {
                RemoveProperty(psProperty);
            }


        }           // end class Thing


        [Serializable]
        public class ThingException : Exception
        {
            public ThingException()
                : base() { }

            public ThingException(string message)
                : base(message) { }

            public ThingException(string format, params object[] args)
                : base(string.Format(format, args)) { }

            public ThingException(string message, Exception innerException)
                : base(message, innerException) { }

            public ThingException(string format, Exception innerException, params object[] args)
                : base(string.Format(format, args), innerException) { }

            //protected ThingException(SerializationInfo info, StreamingContext context)
            //    : base(info, context) { }

        }           // end class ThingException

    }
}
