/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;

using BH.oM.Adapter;

using BH.oM.Adapters.STL;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using BH.Engine.Adapters.STL;
using BH.Engine.Geometry;

namespace BH.Adapter.STL
{
    public partial class STLAdapter : BHoMAdapter
    {
        protected override bool ICreate<T>(IEnumerable<T> objects, ActionConfig actionConfig = null)
        {
            return Create(objects as dynamic);
        }

        private bool Create(IEnumerable<GeometryGroup> geometryGroups)
        {
            foreach(GeometryGroup g in geometryGroups)
            {
                string stlName = g.FileName;
                List<string> geoTxt = new List<string>();
                List<Polyline> polylines = g.Geometry.SelectMany(x => x.IToPolyline()).ToList();

                if (polylines.Any())
                {
                    geoTxt.Add("solid " + stlName + ".stl");
                    foreach (Polyline polyline in polylines)
                    {
                        try
                        {
                            geoTxt.AddRange(polyline.ToSTL(m_stlSettings));
                        }
                        catch (Exception e)
                        {
                            BH.Engine.Base.Compute.RecordError("An error occurred in exporting the STL file. Error is: " + e.ToString());
                            return false;
                        }
                    }
                    geoTxt.Add("endsolid " + stlName.ToString() + ".stl");
                }

                try
                {
                    // write text files
                    File.WriteAllLines(System.IO.Path.Combine(m_stlSettings.Directory, stlName + ".stl"), geoTxt);
                }
                catch (Exception e)
                {
                    BH.Engine.Base.Compute.RecordError("An error occurred in exporting the STL file. Error is: " + e.ToString());
                    return false;
                }
            }
            return true;
        }

        private bool Create(IEnumerable<Polyline> polylines, string fileName = null)
        {
            string stlName = "";
            if (fileName != null)
                stlName = fileName;
            else
                stlName = "Polylines";

            List<string> geoTxt = new List<string>();
            if (polylines.Any())
            {
                geoTxt.Add("solid " + stlName + ".stl");
                foreach (Polyline polyline in polylines)
                {
                    try
                    {
                        geoTxt.AddRange(polyline.ToSTL(m_stlSettings));
                    }
                    catch (Exception e)
                    {
                        BH.Engine.Base.Compute.RecordError("An error occurred in exporting the STL file. Error is: " + e.ToString());
                        return false;
                    }
                }
                geoTxt.Add("endsolid " + stlName.ToString() + ".stl");
            }

            try
            {
                // write text files
                File.WriteAllLines(System.IO.Path.Combine(m_stlSettings.Directory, stlName + ".stl"), geoTxt);
            }
            catch(Exception e)
            {
                BH.Engine.Base.Compute.RecordError("An error occurred in exporting the STL file. Error is: " + e.ToString());
                return false;
            }

            return true;
        }
    }
}



