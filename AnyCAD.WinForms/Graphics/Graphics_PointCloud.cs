﻿using AnyCAD.Forms;
using AnyCAD.Foundation;
using System;
using System.Collections.Generic;
using System.IO;



namespace AnyCAD.Demo.Graphics
{
    class Graphics_PointCloud : TestCase
    {
        static Float32Buffer mPositions;
        static Float32Buffer mColors;
        bool ReadData()
        {
            if (mPositions != null)
                return true;

            string fileName = GetResourcePath("cloud.xyz");
            using (StreamReader reader = File.OpenText(fileName))
            {
                mPositions = new Float32Buffer(0);
                mColors = new Float32Buffer(0);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var items = line.Split(' ');
                    if (items.Length != 6)
                        continue;

                    for (int ii = 0; ii < 3; ++ii)
                    {
                        mPositions.Append(float.Parse(items[ii]));
                    }

                    for (int ii = 3; ii < 6; ++ii)
                    {
                        mColors.Append(float.Parse(items[ii]) / 255.0f);
                    }
                }
            }

            return true;
        }
        public override void Run(RenderControl render)
        {
            if (!ReadData())
                return;

            var materialManager = render.GetMaterialManager();
            var material = materialManager.FindInstance("point-material");
            if (material == null)
            {
                var mt = materialManager.CreateTemplateByName("point-material-template", "basic");
                mt.SetVertexColors(true);
                material = materialManager.Create("point-material", mt);
            }

            var position = BufferAttribute.Create(EnumAttributeSemantic.Position, EnumAttributeComponents.Three, mPositions);
            var color = BufferAttribute.Create(EnumAttributeSemantic.Color, EnumAttributeComponents.Three, mColors);

            BufferGeometry geometry = new BufferGeometry();
            geometry.AddAttribute(position);
            geometry.AddAttribute(color);

            var node = new PrimitiveSceneNode(geometry, EnumPrimitiveType.POINTS);
            node.SetMaterial(material);
            node.SetPickable(false);

            render.ShowSceneNode(node);
        }
    }
}
