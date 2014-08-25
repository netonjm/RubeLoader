using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using CocosSharp;

namespace ChipmunkSharp
{

	public class b2dJsonCustomProperties
	{
		public Dictionary<string, int> m_customPropertyMap_int = new Dictionary<string, int>();
		public Dictionary<string, float> m_customPropertyMap_float = new Dictionary<string, float>();
		public Dictionary<string, string> m_customPropertyMap_string = new Dictionary<string, string>();
		public Dictionary<string, cpVect> m_customPropertyMap_cpVect = new Dictionary<string, cpVect>();
		public Dictionary<string, bool> m_customPropertyMap_bool = new Dictionary<string, bool>();
	}


	public class b2dJson
	{

		protected bool m_useHumanReadableFloats;
		protected int m_simulationPositionIterations;
		protected int m_simulationVelocityIterations;
		protected float m_simulationFPS;

		protected Dictionary<int, CCPhysicsBody> m_indexToBodyMap;
		protected Dictionary<CCPhysicsBody, int?> m_bodyToIndexMap;
		protected Dictionary<CCPhysicsJoint, int?> m_jointToIndexMap;
		protected List<CCPhysicsBody> m_bodies;
		protected List<CCPhysicsJoint> m_joints;
		protected List<b2dJsonImage> m_images;

		//Dicctionary to associate body to name
		protected Dictionary<CCPhysicsBody, string> m_bodyToNameMap;
		protected Dictionary<CCPhysicsShape, string> m_fixtureToNameMap;
		protected Dictionary<CCPhysicsJoint, string> m_jointToNameMap;
		protected Dictionary<b2dJsonImage, string> m_imageToNameMap;


		// This maps an item (Body, Fixture etc) to a set of custom properties.
		// Use null for world properties.
		protected Dictionary<object, b2dJsonCustomProperties> m_customPropertiesMap;

		// These are necessary to know what type of item the entries in the map above
		// are, which is necessary for the getBodyByCustomInt type functions.
		// We could have used a separate map for each item type, but there are many
		// combinations of item type and property type and the overall amount of
		// explicit coding to do becomes very large for no real benefit.
		protected HashSet<CCPhysicsBody> m_bodiesWithCustomProperties;
		protected HashSet<CCPhysicsShape> m_fixturesWithCustomProperties;
		protected HashSet<CCPhysicsJoint> m_jointsWithCustomProperties;
		protected HashSet<b2dJsonImage> m_imagesWithCustomProperties;
		protected HashSet<CCPhysicsWorld> m_worldsWithCustomProperties;


		public bool RenderOrderAscending(b2dJsonImage a, b2dJsonImage b)
		{
			return a.RenderOrder < b.RenderOrder;
		}


		public b2dJson()
			: this(true)
		{

		}

		public b2dJson(bool useHumanReadableFloats)
		{

			if (!useHumanReadableFloats)
			{
				// The floatToHex function is not giving the same results
				// as the original C++ version... not critical so worry about it
				// later.
				Console.WriteLine("Non human readable floats are not implemented yet");
				useHumanReadableFloats = true;
			}

			m_useHumanReadableFloats = useHumanReadableFloats;
			m_simulationPositionIterations = 3;
			m_simulationVelocityIterations = 8;
			m_simulationFPS = 60;

			
			//Array initialization
			m_indexToBodyMap = new Dictionary<int, CCPhysicsBody>();
			m_bodyToIndexMap = new Dictionary<CCPhysicsBody, int?>();
			m_jointToIndexMap = new Dictionary<CCPhysicsJoint, int?>();
			m_bodies = new List<CCPhysicsBody>();
			m_joints = new List<CCPhysicsJoint>();
			m_images = new List<b2dJsonImage>();

			m_bodyToNameMap = new Dictionary<CCPhysicsBody, string>();
			m_fixtureToNameMap = new Dictionary<CCPhysicsShape, string>();
			m_jointToNameMap = new Dictionary<CCPhysicsJoint, string>();
			m_imageToNameMap = new Dictionary<b2dJsonImage, string>();

			m_customPropertiesMap = new Dictionary<object, b2dJsonCustomProperties>();

			m_bodiesWithCustomProperties = new HashSet<CCPhysicsBody>();
			m_fixturesWithCustomProperties = new HashSet<CCPhysicsShape>();
			m_jointsWithCustomProperties = new HashSet<CCPhysicsJoint>();
			m_imagesWithCustomProperties = new HashSet<b2dJsonImage>();
			m_worldsWithCustomProperties = new HashSet<CCPhysicsWorld>();
		}

		//public JObject writeToValue( b2World world) {
		//if (null == world)
		//    return new JObject();

		//return b2j(world);
		//}

		//public String worldToString(b2World world, int indentFactor) {
		//    if (null == world)
		//        return "";

		//    return b2j(world).toString(indentFactor);
		//}


		public void SetBodyName(CCPhysicsBody body, String name)
		{
			m_bodyToNameMap.Add(body, name);
		}

		public void SetFixtureName(CCPhysicsShape fixture, String name)
		{
			m_fixtureToNameMap.Add(fixture, name);
		}

		public void SetJointName(CCPhysicsJoint joint, String name)
		{
			m_jointToNameMap.Add(joint, name);
		}

		public void SetImageName(b2dJsonImage image, String name)
		{
			m_imageToNameMap.Add(image, name);
		}

		public void AddImage(b2dJsonImage image)
		{
			SetImageName(image, image.Name);
		}

		public b2dJsonCustomProperties GetCustomPropertiesForItem(Object item, bool createIfNotExisting)
		{

			if (m_customPropertiesMap.ContainsKey(item))
				return m_customPropertiesMap[item];

			if (!createIfNotExisting)
				return null;

			b2dJsonCustomProperties props = new b2dJsonCustomProperties();
			m_customPropertiesMap[item] = props;

			return props;
		}


		public void SetCustomInt(CCPhysicsBody item, String propertyName, int val)
		{
			m_bodiesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_int.Add(propertyName, val);
		}

		public void SetCustomFloat(CCPhysicsBody item, String propertyName, float val)
		{
			m_bodiesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_float.Add(propertyName, (float)val);
		}

		public void SetCustomString(CCPhysicsBody item, String propertyName, String val)
		{
			m_bodiesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_string.Add(propertyName, val);
		}

		public void SetCustomVector(CCPhysicsBody item, String propertyName, cpVect val)
		{
			m_bodiesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_cpVect.Add(propertyName, val);
		}

		public void SetCustomBool(CCPhysicsBody item, String propertyName, bool val)
		{
			m_bodiesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_bool.Add(propertyName, val);
		}


		public void SetCustomInt(CCPhysicsShape item, String propertyName, int val)
		{
			m_fixturesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_int.Add(propertyName, val);
		}

		public void SetCustomFloat(CCPhysicsShape item, String propertyName, float val)
		{
			m_fixturesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_float.Add(propertyName, (float)val);
		}

		public void SetCustomString(CCPhysicsShape item, String propertyName, String val)
		{
			m_fixturesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_string.Add(propertyName, val);
		}

		public void SetCustomVector(CCPhysicsShape item, String propertyName, cpVect val)
		{
			m_fixturesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_cpVect.Add(propertyName, val);
		}

		public void SetCustomBool(CCPhysicsShape item, String propertyName, bool val)
		{
			m_fixturesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_bool.Add(propertyName, val);
		}

		public int GetCustomInt(object item, string propertyName, int defaultVal)
		{
			b2dJsonCustomProperties props = GetCustomPropertiesForItem(item, false);
			if (props != null)
				return defaultVal;

			int it;
			if (props.m_customPropertyMap_int.TryGetValue(propertyName, out it))
				return it;
			return defaultVal;
		}

		public float GetCustomFloat(object item, string propertyName, float defaultVal)
		{
			b2dJsonCustomProperties props = GetCustomPropertiesForItem(item, false);
			if (props != null)
				return defaultVal;

			float it;
			if (props.m_customPropertyMap_float.TryGetValue(propertyName, out it))
				return it;
			return defaultVal;
		}

		public string GetCustomString(object item, string propertyName, string defaultVal)
		{
			b2dJsonCustomProperties props = GetCustomPropertiesForItem(item, false);
			if (props != null)
				return defaultVal;

			string it;
			if (props.m_customPropertyMap_string.TryGetValue(propertyName, out it))
				return it;
			return defaultVal;
		}

		public cpVect GetCustomVector(object item, string propertyName, cpVect defaultVal)
		{
			b2dJsonCustomProperties props = GetCustomPropertiesForItem(item, false);
			if (props != null)
				return defaultVal;

			cpVect it;
			if (props.m_customPropertyMap_cpVect.TryGetValue(propertyName, out it))
				return it;
			return defaultVal;
		}

		public bool GetCustomBool(object item, string propertyName, bool defaultVal)
		{
			b2dJsonCustomProperties props = GetCustomPropertiesForItem(item, false);
			if (props != null)
				return defaultVal;

			bool it;
			if (props.m_customPropertyMap_bool.TryGetValue(propertyName, out it))
				return it;
			return defaultVal;
		}

		/// //////////////////////////////////////////////////////////////////

		public bool WriteToFile(CCPhysicsWorld world, String filename, int indentFactor, StringBuilder errorMsg)
		{
			if (null == world || null == filename)
				return false;


			using (System.IO.TextWriter writeFile = new StreamWriter(filename))
			{
				try
				{
					writeFile.WriteLine((world).ToString());
				}
				catch (Exception e)
				{
					errorMsg.Append("Error writing JSON to file: " + filename + "  " + e.Message);
					return false;
				}
			}

			return true;
		}

		//public JObject B2n(CCPhysicsWorld world)
		//{
		//	JObject worldValue = new JObject();

		//	m_bodyToIndexMap.Clear();
		//	m_jointToIndexMap.Clear();

		//	VecToJson("gravity", world.GetGravity(), worldValue);

		//	worldValue["allowSleep"] = 1; // world. AllowSleep;

		//	//worldValue["autoClearForces"] = world.;
		//	worldValue["warmStarting"] = 0; // world.GetWarmStarting();
		//	//worldValue["continuousPhysics"] = world.isco;

		//	JArray customPropertyValue = WriteCustomPropertiesToJson(null);
		//	if (customPropertyValue.Count > 0)
		//		worldValue["customProperties"] = customPropertyValue;

		//	int i = 0;

		//	world.EachBody((tmp) =>
		//	{
		//		m_bodyToIndexMap.Add(tmp, i);
		//		worldValue.Add("body", B2n(tmp));
		//	}, null);

		//	world.EachConstraint((tmpJoint) =>
		//	{
		//		if (tmpJoint.GetType() != typeof(cpGearJoint))
		//		{
		//			m_jointToIndexMap.Add(tmpJoint, i);
		//			worldValue.Add("joint", B2n(world));
		//		}
		//	});


		//	// Currently the only efficient way to add images to a Jb2dJson
		//	// is by using the R.U.B.E editor. This code has not been tested,
		//	// but should work ok.
		//	i = 0;
		//	foreach (var image in m_imageToNameMap.Keys)
		//	{
		//		worldValue.Add("image", B2n(image));
		//	}

		//	m_bodyToIndexMap.Clear();
		//	m_jointToIndexMap.Clear();
		//	return worldValue;
		//}

		//JObject B2n(b2dJsonImage image)
		//{
		//	JObject imageValue = new JObject();

		//	if (null != image.Body)
		//		imageValue["body"] = lookupBodyIndex(image.Body);
		//	else
		//		imageValue["body"] = -1;

		//	if (null != image.Name)
		//		imageValue["name"] = image.Name;
		//	if (null != image.File)
		//		imageValue["file"] = image.File;
		//	VecToJson("center", image.Center, imageValue);
		//	FloatToJson("angle", image.Angle, imageValue);
		//	FloatToJson("scale", image.Scale, imageValue);
		//	if (image.Flip)
		//		imageValue["flip"] = true;
		//	FloatToJson("opacity", image.Opacity, imageValue);
		//	imageValue["filter"] = JObject.FromObject(image.Filter); //TODO: Not sure
		//	FloatToJson("renderOrder", image.RenderOrder, imageValue);

		//	//bool defaultColorTint = true;
		//	//for (int i = 0; i < 4; i++)
		//	//{
		//	//	if (image.ColorTint[i] != 255)
		//	//	{
		//	//		defaultColorTint = false;
		//	//		break;
		//	//	}
		//	//}

		//	//if (!defaultColorTint)
		//	//{
		//	//	JArray array = (JArray)imageValue["colorTint"];
		//	//	for (int i = 0; i < 4; i++)
		//	//		array[i] = image.ColorTint[i];
		//	//}

		//	// image->updateCorners();
		//	for (int i = 0; i < 4; i++)
		//		VecToJson("corners", image.Corners[i], imageValue, i);

		//	// image->updateUVs();
		//	for (int i = 0; i < 2 * image.NumPoints; i++)
		//	{
		//		VecToJson("glVertexPointer", image.Points[i], imageValue, i);
		//		VecToJson("glTexCoordPointer", image.UvCoords[i], imageValue, i);
		//	}
		//	for (int i = 0; i < image.NumIndices; i++)
		//		VecToJson("glDrawElements", image.Indices[i], imageValue, i);

		//	JArray customPropertyValue = WriteCustomPropertiesToJson(image);
		//	if (customPropertyValue.Count > 0)
		//		imageValue["customProperties"] = customPropertyValue;

		//	return imageValue;
		//}

		//public JObject B2n(CCPhysicsJoint joint)
		//{
		//	JObject jointValue = new JObject();

		//	String jointName = GetJointName(joint);
		//	if (null != jointName)
		//		jointValue["name"] = jointName;

		//	int bodyIndexA = lookupBodyIndex(joint.GetBodyA());
		//	int bodyIndexB = lookupBodyIndex(joint.GetBodyB());
		//	jointValue["bodyA"] = bodyIndexA;
		//	jointValue["bodyB"] = bodyIndexB;

		//	//if (joint.GetCollideConnected())
		//	//	jointValue["collideConnected"] = true;

		//	CCPhysicsBody bodyA = joint.GetBodyA();
		//	CCPhysicsBody bodyB = joint.GetBodyB();

		//	// why do Joint.getAnchor methods need to take an argOut style
		//	// parameter!?
		//	cpVect tmpAnchor;

		//	if (joint.GetType() == typeof(cpPinJoint))
		//	{
		//		//case CCPhysicsJointType.e_revoluteJoint:
		//		//{
		//		jointValue["type"] = "revolute";

		//		cpPinJoint revoluteJoint = (cpPinJoint)joint;
		//		tmpAnchor = revoluteJoint.GetAnchorA();
		//		//VecToJson("anchorA", bodyA.GetLocalPoint(tmpAnchor), jointValue);

		//		//tmpAnchor = revoluteJoint.GetAnchorB();
		//		//VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);


		//		//FloatToJson("refAngle", bodyB.GetAngle() - bodyA.GetAngle() - revoluteJoint.GetRestAngle(), jointValue);
		//		//FloatToJson("jointSpeed", revoluteJoint.GetJointSpeed(), jointValue);
		//		//jointValue["enableLimit"] = revoluteJoint.IsLimitEnabled();
		//		//FloatToJson("lowerLimit", revoluteJoint.GetLowerLimit(), jointValue);
		//		//FloatToJson("upperLimit", revoluteJoint.GetUpperLimit(), jointValue);
		//		//jointValue["enableMotor"] = revoluteJoint.IsMotorEnabled();
		//		//FloatToJson("motorSpeed", revoluteJoint.GetMotorSpeed(), jointValue);
		//		//FloatToJson("maxMotorTorque", revoluteJoint. GetMaxMotorTorque(), jointValue);

		//	}
		//	else if (joint.GetType() == typeof(cpPinJoint)) // CCPhysicsJointType.e_prismaticJoint:
		//	{
		//		jointValue["type"] = "prismatic";

		//		//b2PrismaticJoint prismaticJoint = (b2PrismaticJoint)joint;
		//		//tmpAnchor = prismaticJoint.GetAnchorA();
		//		//VecToJson("anchorA", bodyA.GetLocalPoint(tmpAnchor), jointValue);

		//		//tmpAnchor = prismaticJoint.GetAnchorB();
		//		//VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);

		//		//VecToJson("localAxisA", prismaticJoint.GetLocalXAxisA(), jointValue);
		//		//FloatToJson("refAngle", prismaticJoint.GetReferenceAngle(), jointValue);
		//		//jointValue["enableLimit"] = prismaticJoint.IsLimitEnabled();
		//		//FloatToJson("lowerLimit", prismaticJoint.GetLowerLimit(), jointValue);
		//		//FloatToJson("upperLimit", prismaticJoint.GetUpperLimit(), jointValue);
		//		//jointValue["enableMotor"] = prismaticJoint.IsMotorEnabled();
		//		//FloatToJson("maxMotorForce", prismaticJoint.GetMaxMotorForce(), jointValue);
		//		//FloatToJson("motorSpeed", prismaticJoint.GetMotorSpeed(), jointValue);
		//	}
		//	else if (joint.GetType() == typeof(cpSlideJoint)) // CCPhysicsJointType.e_distanceJoint:
		//	{
		//		jointValue["type"] = "distance";

		//		cpSlideJoint distanceJoint = (cpSlideJoint)joint;
		//		tmpAnchor = distanceJoint.GetAnchorA();
		//		//VecToJson("anchorA", bodyA.GetLocalPoint(tmpAnchor), jointValue);
		//		tmpAnchor = distanceJoint.GetAnchorB();
		//		//VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);

		//		//TODO: UNKOWS METHODS
		//		//FloatToJson("length", distanceJoint.GetLenght(), jointValue);
		//		//FloatToJson("frequency", distanceJoint.GetFrequency(), jointValue);
		//		//FloatToJson("dampingRatio", distanceJoint.GetDampingRatio(), jointValue);
		//	}

		//	else if (joint.GetType() == typeof(cpSlideJoint)) //case CCPhysicsJointType.e_pulleyJoint:
		//	{
		//		jointValue["type"] = "pulley";

		//		//b2PulleyJoint pulleyJoint = (b2PulleyJoint)joint;
		//		//VecToJson("groundAnchorA", pulleyJoint.GetGroundAnchorA(), jointValue);
		//		//VecToJson("groundAnchorB", pulleyJoint.GetGroundAnchorB(), jointValue);
		//		//tmpAnchor = pulleyJoint.GetAnchorA();
		//		//VecToJson("anchorA", bodyA.GetLocalPoint(tmpAnchor), jointValue);
		//		//FloatToJson("lengthA", (pulleyJoint.GetGroundAnchorA() - tmpAnchor).Length, jointValue);
		//		//tmpAnchor = pulleyJoint.GetAnchorB();
		//		//VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);
		//		//FloatToJson("lengthB", (pulleyJoint.GetGroundAnchorB() - tmpAnchor).Length, jointValue);
		//		//FloatToJson("ratio", pulleyJoint.GetRatio(), jointValue);
		//	}
		//	else if (joint.GetType() == typeof(cpPivotJoint)) //case CCPhysicsJointType.e_mouseJoint:
		//	{
		//		jointValue["type"] = "mouse";

		//		cpPivotJoint mouseJoint = (cpPivotJoint)joint;

		//		//VecToJson("target", mouseJoint. GetTarget(), jointValue);
		//		tmpAnchor = mouseJoint.GetAnchorB();
		//		VecToJson("anchorB", tmpAnchor, jointValue);
		//		FloatToJson("maxForce", mouseJoint.GetMaxForce(), jointValue);
		//		//FloatToJson("frequency", mouseJoint.GetFrequency(), jointValue);
		//		FloatToJson("dampingRatio", mouseJoint.GetDamping(), jointValue);
		//	}
		//	else if (joint.GetType() == typeof(cpGearJoint))	//case CCPhysicsJointType.e_gearJoint:
		//	{
		//		jointValue["type"] = "gear";
		//		cpGearJoint gearJoint = (cpGearJoint)joint;
		//		//int jointIndex1 = lookupJointIndex(gearJoint.GetGrooveA()); // GetJoint1());
		//		//int jointIndex2 = lookupJointIndex(gearJoint.GetGrooveB()); // GetJoint2());
		//		//jointValue["joint1"] = jointIndex1;
		//		//jointValue["joint2"] = jointIndex2;
		//		jointValue["ratio"] = gearJoint.GetRatio();
		//	}
		//	else if (joint.GetType() == typeof(cpPivotJoint))	//case CCPhysicsJointType.e_wheelJoint:
		//	{
		//		jointValue["type"] = "wheel";
		//		cpPivotJoint wheelJoint = (cpPivotJoint)joint;
		//		tmpAnchor = wheelJoint.GetAnchorA();
		//		//VecToJson("anchorA", tmpAnchor, jointValue);
		//		//tmpAnchor = wheelJoint.GetAnchorB();
		//		//VecToJson("anchorB", tmpAnchor, jointValue);
		//		////TODO: UNKNOWN METHOD
		//		////VecToJson("localAxisA", wheelJoint.GetLocalAxisA(), jointValue);
		//		//jointValue["enableMotor"] = wheelJoint.IsMotorEnabled();
		//		//FloatToJson("motorSpeed", wheelJoint.GetMotorSpeed(), jointValue);
		//		//FloatToJson("maxMotorTorque", wheelJoint.GetMaxMotorTorque(), jointValue);
		//		//FloatToJson("springFrequency", wheelJoint.GetSpringFrequencyHz(), jointValue);
		//		//FloatToJson("springDampingRatio", wheelJoint.GetSpringDampingRatio(), jointValue);

		//	}
		//	//break;
		//	else if (joint.GetType() == typeof(cpGearJoint))  //case CCPhysicsJointType.e_weldJoint:
		//	{
		//		jointValue["type"] = "weld";

		//		cpGearJoint weldJoint = (cpGearJoint)joint;
		//		tmpAnchor = weldJoint.GetAnchorA();
		//		//VecToJson("anchorA", bodyA.GetLocalPoint(tmpAnchor), jointValue);
		//		//tmpAnchor = weldJoint.GetAnchorB();
		//		//VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);
		//		//FloatToJson("refAngle", weldJoint.ReferenceAngle, jointValue);
		//		//FloatToJson("frequency", weldJoint.Frequency, jointValue);
		//		//FloatToJson("dampingRatio", weldJoint.DampingRatio, jointValue);
		//	}
		//	else if (joint.GetType() == typeof(cpPivotJoint)) //case CCPhysicsJointType.e_frictionJoint:
		//	{
		//		jointValue["type"] = "friction";

		//		cpPivotJoint frictionJoint = (cpPivotJoint)joint;
		//		tmpAnchor = frictionJoint.GetAnchorA();
		//		//VecToJson("anchorA", bodyA.GetLocalPoint(tmpAnchor), jointValue);
		//		//tmpAnchor = frictionJoint.GetAnchorB();
		//		//VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);
		//		//FloatToJson("maxForce", frictionJoint.GetMaxForce(), jointValue);
		//		//FloatToJson("maxTorque", frictionJoint.GetMaxTorque(), jointValue);
		//	}


		//	else if (joint.GetType() == typeof(cpSlideJoint)) //case CCPhysicsJointType.e_ropeJoint:
		//	{
		//		// Rope joints are apparently not implemented in JBox2D yet, but
		//		// when they are, commenting out the following section should work.

		//		jointValue["type"] = "rope";

		//		cpSlideJoint ropeJoint = (cpSlideJoint)joint;
		//		tmpAnchor = ropeJoint.GetAnchorA();
		//		//VecToJson("anchorA", bodyA.GetLocalPoint(tmpAnchor), jointValue);
		//		//tmpAnchor = ropeJoint.GetAnchorB();
		//		//VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);
		//		//FloatToJson("maxLength", ropeJoint.GetMaxLength(), jointValue);

		//	}
		//	else
		//	{
		//		Console.WriteLine("Unknown joint type : " + joint.GetType());
		//	}

		//	//case CCPhysicsJointType.Motor:
		//	//{
		//	//    jointValue["type"] = "motor";
		//	//    MotorJoint motor = (MotorJoint)joint;
		//	//    tmpAnchor = motor.WorldAnchorA;
		//	//    VecToJson("anchorA", motor.LinearOffset, jointValue);
		//	//    //tmpAnchor = motor.WorldAnchorB;
		//	//    //VecToJson("anchorB", bodyB.GetLocalPoint(tmpAnchor), jointValue);
		//	//    FloatToJson("maxForce", motor.MaxForce, jointValue);
		//	//    FloatToJson("maxTorque", motor.MaxTorque, jointValue);
		//	//    FloatToJson("refAngle", motor.AngularOffset, jointValue);
		//	//}
		//	//break;


		//	JArray customPropertyValue = WriteCustomPropertiesToJson(joint);
		//	if (customPropertyValue.Count > 0)
		//		jointValue["customProperties"] = customPropertyValue;

		//	return jointValue;
		//}

		//public JObject B2n(CCPhysicsBody body)
		//{
		//	JObject bodyValue = new JObject();

		//	string bodyName = GetBodyName(body);
		//	if (null != bodyName)
		//		bodyValue["name"] = bodyName;

		//	switch (body.bodyType)
		//	{
		//		case CCPhysicsBodyType.STATIC:
		//			bodyValue["type"] = 0;
		//			break;
		//		case CCPhysicsBodyType.KINEMATIC:
		//			bodyValue["type"] = 1;
		//			break;
		//		case CCPhysicsBodyType.DYNAMIC:
		//			bodyValue["type"] = 2;
		//			break;
		//	}

		//	VecToJson("position", body.GetPosition(), bodyValue);
		//	FloatToJson("angle", body.GetAngle(), bodyValue);

		//	VecToJson("linearVelocity", body.GetVelocity(), bodyValue);
		//	FloatToJson("angularVelocity", body.GetAngularVelocity(), bodyValue);

		//	//if (body.LinearDamping != 0)
		//	//	FloatToJson("linearDamping", body.LinearDamping, bodyValue);
		//	//if (body.AngularDamping != 0)
		//	//	FloatToJson("angularDamping", body.AngularDamping, bodyValue);
		//	//if (body.GravityScale != 1)
		//	//	FloatToJson("gravityScale", body.GravityScale, bodyValue);

		//	//if (body.IsBullet())
		//	//	bodyValue["bullet"] = true;
		//	//if (!body.IsSleepingAllowed())
		//	//	bodyValue["allowSleep"] = false;
		//	//if (body.IsAwake())
		//	//	bodyValue["awake"] = true;
		//	//if (!body.IsActive())
		//	//	bodyValue["active"] = false;
		//	//if (body.IsFixedRotation())
		//	//	bodyValue["fixedRotation"] = true;

		//	b2MassData massData = new b2MassData();
		//	massData = body.GetMassData();

		//	if (massData.mass != 0)
		//		FloatToJson("massData-mass", massData.mass, bodyValue);
		//	if (massData.center.x != 0 || massData.center.y != 0)
		//		VecToJson("massData-center", body.cog, bodyValue);
		//	if (massData.I != 0)
		//	{
		//		FloatToJson("massData-I", massData.I, bodyValue);
		//	}

		//	//int i = 0;
		//	JArray arr = new JArray();
		//	CCPhysicsBody tmp = body;
		//	while (tmp != null)
		//	{
		//		bodyValue.Add("fixture", B2n(tmp));
		//		tmp = body.nodeNext;
		//	}

		//	bodyValue["fixture"] = arr;

		//	JArray customPropertyValue = WriteCustomPropertiesToJson(body);
		//	if (customPropertyValue.Count > 0)
		//		bodyValue["customProperties"] = customPropertyValue;

		//	return bodyValue;
		//}

		//public JObject B2n(CCPhysicsShape shape)
		//{
		//	JObject fixtureValue = new JObject();



		//	String fixtureName = GetFixtureName(shape);
		//	if (null != fixtureName)
		//		fixtureValue["name"] = fixtureName;

		//	//if (fixture.Restitution != 0)
		//	//	FloatToJson("restitution", fixture.Restitution, fixtureValue);
		//	if (shape.GetFriction() != 0)
		//		FloatToJson("friction", shape.Friction, fixtureValue);
		//	//if (fixture. Density != 0)
		//	//	FloatToJson("density", fixture.Density, fixtureValue);
		//	if (shape.sensor)
		//		fixtureValue["sensor"] = true;

		//	if ((int)shape.Filter.categoryBits != 0x0001)
		//		fixtureValue["filter-categoryBits"] = (int)shape.Filter.categoryBits;
		//	if ((int)shape.Filter.maskBits != 0xffff)
		//		fixtureValue["filter-maskBits"] = (int)shape.Filter.maskBits;
		//	if (shape.Filter.groupIndex != 0)
		//		fixtureValue["filter-groupIndex"] = shape.Filter.groupIndex;

		//	switch (shape.shapeType)
		//	{
		//		case CCPhysicsShapeType.Circle:
		//			{
		//				cpCircleShape circle = (cpCircleShape)shape;
		//				JObject shapeValue = new JObject();
		//				FloatToJson("radius", circle.Radius, shapeValue);
		//				VecToJson("center", circle.Position, shapeValue);
		//				fixtureValue["circle"] = shapeValue;
		//			}
		//			break;
		//		case CCPhysicsShapeType.Segment:
		//			{
		//				cpSegmentShape edge = (cpSegmentShape)shape;
		//				JObject shapeValue = new JObject();
		//				VecToJson("vertex1", edge.Vertex1, shapeValue);
		//				VecToJson("vertex2", edge.Vertex2, shapeValue);
		//				if (edge.HasVertex0)
		//					shapeValue["hasVertex0"] = true;
		//				if (edge.HasVertex3)
		//					shapeValue["hasVertex3"] = true;
		//				if (edge.HasVertex0)
		//					VecToJson("vertex0", edge.Vertex0, shapeValue);
		//				if (edge.HasVertex3)
		//					VecToJson("vertex3", edge.Vertex3, shapeValue);
		//				fixtureValue["edge"] = shapeValue;
		//			}
		//			break;
		//		//case b2ShapeType.e_chain:
		//		//	{
		//		//		b2ChainShape chain = (b2ChainShape)shape;
		//		//		JObject shapeValue = new JObject();
		//		//		int count = chain.Vertices.Length;
		//		//		for (int i = 0; i < count; ++i)
		//		//			VecToJson("vertices", chain.Vertices[i], shapeValue, i);
		//		//		if (chain.PrevVertex != null && chain.PrevVertex != cpVect.Zero)
		//		//		{
		//		//			shapeValue["hasPrevVertex"] = true;
		//		//			VecToJson("prevVertex", chain.PrevVertex, shapeValue);
		//		//		}
		//		//		if (chain.NextVertex != null && chain.NextVertex != cpVect.Zero)
		//		//		{
		//		//			shapeValue["hasNextVertex"] = true;
		//		//			VecToJson("nextVertex", chain.NextVertex, shapeValue);
		//		//		}

		//		//		fixtureValue["chain"] = shapeValue;
		//		//	}
		//		//	break;
		//		case CCPhysicsShapeType.Polygon:
		//			{
		//				cpPolyShape poly = (cpPolyShape)shape;
		//				JObject shapeValue = new JObject();
		//				int vertexCount = poly.Count;
		//				for (int i = 0; i < vertexCount; ++i)
		//					VecToJson("vertices", poly.Vertices[i], shapeValue, i);
		//				fixtureValue["polygon"] = shapeValue;
		//			}
		//			break;
		//		default:
		//			Console.WriteLine("Unknown shape type : " + shape.shapeType);
		//			break;
		//	}

		//	JArray customPropertyValue = WriteCustomPropertiesToJson(shape);
		//	if (customPropertyValue.Count > 0)
		//		fixtureValue["customProperties"] = customPropertyValue;

		//	return fixtureValue;
		//}

		public void VecToJson(String name, float v, JObject value, int index)
		{
			if (index > -1)
			{
				if (m_useHumanReadableFloats)
				{
					JArray array = (JArray)value[name];
					array[index] = v;
				}
				else
				{
					JArray array = (JArray)value[name];
					if (v == 0)
						array[index] = 0;
					else if (v == 1)
						array[index] = 1;
					else
						array[index] = FloatToHex(v);
				}
			}
			else
				FloatToJson(name, v, value);
		}

		public void VecToJson(string name, cpVect vec, JObject value)
		{
			VecToJson(name, vec, value, -1);
		}

		public void VecToJson(string name, cpVect vec, JObject value, int index)
		{

			if (index > -1)
			{
				if (m_useHumanReadableFloats)
				{
					bool alreadyHadArray = value[name] != null;
					JArray arrayX = alreadyHadArray ? (JArray)value[name]["x"] : new JArray();
					JArray arrayY = alreadyHadArray ? (JArray)value[name]["y"] : new JArray();
					arrayX.Add(vec.x);
					arrayY.Add(vec.y);
					if (!alreadyHadArray)
					{
						JObject subValue = new JObject();
						subValue["x"] = arrayX;
						subValue["y"] = arrayY;
						value[name] = subValue;
					}
				}
				else
				{
					bool alreadyHadArray = value[name] != null;
					JArray arrayX = alreadyHadArray ? (JArray)value[name]["x"] : new JArray();
					JArray arrayY = alreadyHadArray ? (JArray)value[name]["y"] : new JArray();
					if (vec.x == 0)
						arrayX.Add(0);
					else if (vec.y == 1)
						arrayX.Add(1);
					else
						arrayX.Add(FloatToHex(vec.x));
					if (vec.y == 0)
						arrayY.Add(0);
					else if (vec.y == 1)
						arrayY.Add(1);
					else
						arrayY.Add(FloatToHex(vec.y));
					if (!alreadyHadArray)
					{
						JObject subValue = new JObject();
						subValue["x"] = arrayX;
						subValue["y"] = arrayY;
						value[name] = subValue;
					}
				}
			}
			else
			{
				if (vec.x == 0 && vec.y == 0)
					value[name] = 0;// cut down on file space for common values
				else
				{
					JObject vecValue = new JObject();
					FloatToJson("x", vec.x, vecValue);
					FloatToJson("y", vec.y, vecValue);
					value[name] = vecValue;
				}
			}

		}

		protected JArray WriteCustomPropertiesToJson(Object item)
		{
			JArray customPropertiesValue = new JArray();

			if (null == item)
				return customPropertiesValue;

			b2dJsonCustomProperties props = GetCustomPropertiesForItem(item, false);

			if (props == null)
				return customPropertiesValue;


			foreach (var customProp in props.m_customPropertyMap_int)
			{
				KeyValuePair<string, int> pair = (KeyValuePair<string, int>)customProp;
				JObject proValue = new JObject();
				proValue["name"] = pair.Key;
				proValue["int"] = pair.Value;
				customPropertiesValue.Add(proValue);
			}



			foreach (var customProp in props.m_customPropertyMap_float)
			{
				KeyValuePair<string, float> pair = (KeyValuePair<string, float>)customProp;
				JObject proValue = new JObject();
				proValue["name"] = pair.Key;
				proValue["float"] = pair.Value;
				customPropertiesValue.Add(proValue);
			}




			foreach (var customProp in props.m_customPropertyMap_string)
			{
				KeyValuePair<string, string> pair = (KeyValuePair<string, string>)customProp;
				JObject proValue = new JObject();
				proValue["name"] = pair.Key;
				proValue["string"] = pair.Value;
				customPropertiesValue.Add(proValue);
			}



			foreach (var customProp in props.m_customPropertyMap_cpVect)
			{
				KeyValuePair<string, cpVect> pair = (KeyValuePair<string, cpVect>)customProp;
				JObject proValue = new JObject();
				proValue["name"] = pair.Key;
				VecToJson("vec2", pair.Value, proValue);
				customPropertiesValue.Add(proValue);
			}



			foreach (var customProp in props.m_customPropertyMap_bool)
			{
				KeyValuePair<string, bool> pair = (KeyValuePair<string, bool>)customProp;
				JObject proValue = new JObject();
				proValue["name"] = pair.Key;
				proValue["bool"] = pair.Value;
				customPropertiesValue.Add(proValue);
			}

			return customPropertiesValue;
		}



		CCPhysicsBody lookupBodyFromIndex(int index)
		{
			if (m_indexToBodyMap.ContainsKey(index))
				return m_indexToBodyMap[index];
			else
				return null;
		}

		protected int lookupBodyIndex(CCPhysicsBody body)
		{
			int? val = m_bodyToIndexMap[body];
			if (null != val)
				return val.Value;
			else
				return -1;
		}

		protected int lookupJointIndex(CCPhysicsJoint joint)
		{
			int? val = m_jointToIndexMap[joint];
			if (null != val)
				return val.Value;
			else
				return -1;
		}

		public String GetImageName(b2dJsonImage image)
		{
			if (m_imageToNameMap.ContainsKey(image))
				return m_imageToNameMap[image];
			return null;
		}

		public String GetJointName(CCPhysicsJoint joint)
		{

			if (m_jointToNameMap.ContainsKey(joint))
				return m_jointToNameMap[joint];
			return null;
		}

		public string GetBodyName(CCPhysicsBody body)
		{
			if (m_bodyToNameMap.ContainsKey(body))
				return m_bodyToNameMap[body];
			return null;
		}

		public String GetFixtureName(CCPhysicsShape fixture)
		{
			if (m_fixtureToNameMap.ContainsKey(fixture))
				return m_fixtureToNameMap[fixture];
			return null;
		}

		public void FloatToJson(String name, float f, JObject value)
		{
			// cut down on file space for common values
			if (f == 0)
				value.Add(name, 0);
			else if (f == 1)
				value.Add(name, 1);
			else
			{
				if (m_useHumanReadableFloats)
					value.Add(name, f);
				else
					value.Add(name, FloatToHex(f));
			}
		}

		// Convert a float argument to a byte array and display it. 
		public string FloatToHex(float argument)
		{
			//no usar todavia
			byte[] byteArray = BitConverter.GetBytes(argument);
			string formatter = "{0,16:E7}{1,20}";
			var res = string.Format(formatter, argument,
									BitConverter.ToString(byteArray));
			return res;
		}

		public CCPhysicsBody[] GetBodiesByName(string name)
		{
			List<CCPhysicsBody> bodies = new List<CCPhysicsBody>();
			foreach (var item in m_bodyToNameMap.Keys)
			{
				if (m_bodyToNameMap[item] == name)
					bodies.Add(item);
			}
			return bodies.ToArray();
		}


		public CCPhysicsShape[] GetFixturesByName(String name)
		{
			List<CCPhysicsShape> fixtures = new List<CCPhysicsShape>();

			foreach (var item in m_fixtureToNameMap.Keys)
			{
				if (m_fixtureToNameMap[item] == name)
					fixtures.Add(item);
			}
			return fixtures.ToArray();
		}

		public CCPhysicsJoint[] GetJointsByName(String name)
		{
			List<CCPhysicsJoint> joints = new List<CCPhysicsJoint>();
			//
			foreach (var item in m_jointToNameMap.Keys)
			{
				if (m_jointToNameMap[item] == name)
					joints.Add(item);
			}
			return joints.ToArray();
		}


		public b2dJsonImage[] GetImagesByName(string name)
		{
			List<b2dJsonImage> images = new List<b2dJsonImage>();
			foreach (var item in m_imageToNameMap.Keys)
			{
				if (m_imageToNameMap[item] == name)
					images.Add(item);
			}
			return images.ToArray();
		}


		public List<CCPhysicsBody> GetAllBodies()
		{
			return m_bodies;
		}

		public b2dJsonImage[] GetAllImages()
		{
			return (b2dJsonImage[])m_images.ToArray();
		}

		public CCPhysicsBody GetBodyByName(string name)
		{
			foreach (var item in m_bodyToNameMap.Keys)
			{
				if (m_bodyToNameMap[item] == name)
					return item;
			}
			return null;

		}

		public CCPhysicsShape GetFixtureByName(string name)
		{
			foreach (var item in m_fixtureToNameMap.Keys)
			{
				if (m_fixtureToNameMap[item] == name)
					return item;
			}
			return null;
		}

		public CCPhysicsJoint GetJointByName(String name)
		{
			foreach (var item in m_jointToNameMap.Keys)
			{
				if (m_jointToNameMap[item] == name)
					return item;
			}
			return null;
		}

		public b2dJsonImage GetImageByName(String name)
		{
			foreach (var item in m_imageToNameMap.Keys)
			{
				if (m_imageToNameMap[item] == name)
					return item;
			}
			return null;
		}

		#region salida

		public  bool ReadFromFile(string filename, CCNode node)
		{

			if (null == filename)
			{
				Trace.TraceError("Filename is blank : " + filename );
				return false;
			}

			if (node.Scene == null)
			{
				Trace.TraceError("Scene is not asigned to the Node selected");
				return false;
			}

			if (node.Scene.PhysicsWorld == null)
			{
				Trace.TraceError("Physics are not enabled on this scene");
				return false;
			}

			string str = "";
			try
			{
				System.IO.TextReader readFile = new StreamReader(filename);
				str = readFile.ReadToEnd();
				readFile.Close();
				readFile = null;
				JObject worldValue = JObject.Parse(str);
				return j2cpSpace(worldValue, node);
			}
			catch (IOException ex)
			{
				Trace.TraceError("Error reading file: " + filename + ex.Message);
				
			}

			return false;
		}

        public bool j2cpSpace(JObject worldValue, CCNode node)
		{

			m_bodies.Clear();
			m_simulationPositionIterations = 3;
			m_simulationVelocityIterations = 8;
			m_simulationFPS = 60;

			JToken positionIterations;
			if (worldValue.TryGetValue("positionIterations", out positionIterations))
			{
				m_simulationPositionIterations = (int) positionIterations;
			}


			JToken velocityIterations;
			if (worldValue.TryGetValue("velocityIterations", out velocityIterations))
			{
				m_simulationVelocityIterations = (int)velocityIterations;
			}

			JToken stepsPerSecond;
			if (worldValue.TryGetValue("stepsPerSecond", out stepsPerSecond))
			{
				m_simulationFPS = (float)stepsPerSecond;
			}


			float sleepTimeThreshold = 0.5f;

			JToken allowSleep;
            if (worldValue.TryGetValue("allowSleep", out allowSleep))
			{
				if (! (bool) allowSleep)
				sleepTimeThreshold = cp.Infinity;
			}

			CCPhysicsWorld space = node.Scene.PhysicsWorld;//.Space;

			space.Iterations = m_simulationVelocityIterations;
			space.Gravity = jsonToPoint("gravity",worldValue);
			space.SleepTimeThreshold = sleepTimeThreshold;
			
			readCustomPropertiesFromJson(space, worldValue);

			int i = 0;
			JArray bodyValues = (JArray)worldValue["body"];
			if (null != bodyValues)
			{
				int numBodyValues = bodyValues.Count;
				for (i = 0; i < numBodyValues; i++)
				{
					JObject bodyValue = (JObject)bodyValues[i];
                    CCPhysicsBody body = j2cpBody(bodyValue);
                    //massData.I = jsonToFloat("massData-I", bodyValue);
                    //body.SetMassData(massData);
                    space.AddBody(body);
					readCustomPropertiesFromJson(body, bodyValue);
					m_bodies.Add(body);
					m_indexToBodyMap.Add(i, body);
				}
			}

			// need two passes for joints because gear joints reference other joints
			JArray jointValues = (JArray)worldValue["joint"];
			if (null != jointValues)
			{
				int numJointValues = jointValues.Count;
				for (i = 0; i < numJointValues; i++)
				{
					JObject jointValue = (JObject)jointValues[i];
					if (jointValue["type"].ToString() != "gear")
					{
                        CCPhysicsJoint joint = j2cpConstraint(space, jointValue);
						readCustomPropertiesFromJson(joint, jointValue);
						m_joints.Add(joint);
					}
				}
                //for (i = 0; i < numJointValues; i++)
                //{
                //    JObject jointValue = (JObject)jointValues[i];
                //    if (jointValue["type"].ToString() == "gear")
                //    {
                //        CCPhysicsJoint joint = j2CCPhysicsJoint(space, jointValue);
                //        readCustomPropertiesFromJson(joint, jointValue);
                //        m_joints.Add(joint);
                //    }
                //}
			}
			i = 0;
			JArray imageValues = (JArray)worldValue["image"];
			if (null != imageValues)
			{
				int numImageValues = imageValues.Count;
				for (i = 0; i < numImageValues; i++)
				{
					JObject imageValue = (JObject)imageValues[i];
					b2dJsonImage image = j2b2dJsonImage(imageValue);
					readCustomPropertiesFromJson(image, imageValue);
					m_images.Add(image);
				}
			}
			return true;
		}

        public CCPhysicsBody j2cpBody( JObject bodyValue)
		{

			CCPhysicsBody body = new CCPhysicsBody();
            SetBodyTypeFromInt(body, (int)bodyValue.GetValue("type"));
			//int i = 0;

			JArray fixtureValues = (JArray)bodyValue["fixture"];
			if (null != fixtureValues)
			{
                List<CCPhysicsShape> shape;
				int numFixtureValues = fixtureValues.Count;
				for (int i = 0; i < numFixtureValues; i++)
				{
					JObject fixtureValue = (JObject)fixtureValues[i];
                    shape = j2cpShape(fixtureValue, body.BodyType);
                    readCustomPropertiesFromJson(shape, fixtureValue);
                    body.AddShape(shape,true);
				}
			}

			body.Position = jsonToPoint("position", bodyValue);
			body.SetAngle(jsonToFloat("angle", bodyValue));
			body.SetVelocity(jsonToPoint("linearVelocity", bodyValue));
			body.SetAngularVelocity( jsonToFloat("angularVelocity", bodyValue));
			//body. linearDamping = jsonToFloat("linearDamping", bodyValue, -1, 0);
			//body.set angularDamping = jsonToFloat("angularDamping", bodyValue, -1, 0);
			//body.gravityScale = jsonToFloat("gravityScale", bodyValue, -1, 1);

			//body.allowSleep = bodyValue["allowSleep"] == null ? false : (bool)bodyValue["allowSleep"];
			//body.awake =;
			var awake = bodyValue["awake"] == null ? false : (bool)bodyValue["awake"];
			if (!awake)
				body.Body.Sleep();

			var fixedRotation = bodyValue["fixedRotation"] == null ? false : (bool)bodyValue["fixedRotation"];
			if (fixedRotation)
				body.Moment = cp.Infinity;

			//body.bullet = bodyValue["bullet"] == null ? false : (bool)bodyValue["bullet"];

			//bodyDef.active = bodyValue["active"] == null ? false : (bool)bodyValue["active"];

			//body.active = true;

		
			String bodyName = bodyValue["name"] == null ? "" : (string)bodyValue["active"];
			if (!string.IsNullOrEmpty(bodyName))
				SetBodyName(body, bodyName);


			// may be necessary if user has overridden mass characteristics
			//b2MassData massData = new b2MassData();
			if (body.BodyType == cpBodyType.DYNAMIC)
			body.Body.SetMass(jsonToFloat("massData-mass", bodyValue));
			//massData.mass = ;
			body.Body.SetCenterOfGravity(jsonToVec("massData-center", bodyValue));
			//massData.center =;
			
			
			return body;
		}


		// REVISADO =====================================================================

		public void SetBodyTypeFromInt(CCPhysicsBody body, int type)
		{
			switch (type)
			{
				case 0:
					body.BodyType = cpBodyType.STATIC;
					//body = CCPhysicsBody.NewStatic(); // CCPhysicsBodyType.b2_staticBody;
					break;
				case 1:
					//body = CCPhysicsBody.NewKinematic(); // CCPhysicsBodyType.b2_kinematicBody;
					body.BodyType = cpBodyType.KINEMATIC;
					break;
				//default:
				//	body = CCPhysicsBody.New(1,1); // = CCPhysicsBodyType.b2_dynamicBody;
				//	break;
			}
		}

        List<CCPhysicsShape> j2cpShape(JObject fixtureValue, cpBodyType bodyType)
        {
            if (null == fixtureValue)
                return null;

            float restitution = jsonToFloat("restitution", fixtureValue);
            float friction = jsonToFloat("friction", fixtureValue);
            float density = jsonToFloat("density", fixtureValue);

            bool isSensor = fixtureValue["sensor"] == null ? false : (bool)fixtureValue["sensor"];

            ushort categoryBits = (fixtureValue["filter-categoryBits"] == null) ? (ushort)0x0001 : (ushort)fixtureValue["filter-categoryBits"];

            ushort maskBits = fixtureValue["filter-maskBits"] == null ? (ushort)0xffff : (ushort)fixtureValue["filter-maskBits"];

            short groupIndex = fixtureValue["filter-groupIndex"] == null ? (short)0 : (short)fixtureValue["filter-groupIndex"];

            List<CCPhysicsShape> shapes = new List<CCPhysicsShape>();


            //CCPhysicsShape shape = null;

            if (null != fixtureValue["circle"])
            {
                JObject circleValue = (JObject)fixtureValue["circle"];

                CCPoint center = jsonToPoint("center", circleValue);

                float radius = jsonToFloat("radius", circleValue);

                CCPhysicsShape shape = new CCPhysicsShape(radius, CCPhysicsMaterial.PHYSICSSHAPE_MATERIAL_DEFAULT, center);
                shapes.Add(shape);

                float area = cp.AreaForCircle(0, radius);
                float mass = density * area;

                shape.Moment = cp.MomentForCircle(mass, 0, radius, new cpVect(center.X, center.Y));

            

            }
            else if (null != fixtureValue["edge"])
            {

                JObject edgeValue = (JObject)fixtureValue["edge"];
                cpVect vertex1 = jsonToVec("vertex1", edgeValue);
                cpVect vertex2 = jsonToVec("vertex1", edgeValue);
                float radius = 0;

                CCPhysicsShape shape = new CCPhysicsShapeEdgeSegment(new CCPoint(vertex1.x, vertex1.y), new CCPoint(vertex2.x, vertex2.y), radius);
                shapes.Add(shape);
                //SetBodyTypeFromInt(shape, type);

                if (bodyType != cpBodyType.STATIC)
                {

                    float area = cp.AreaForSegment(vertex1, vertex2, radius);
                    float mass = density * area;

                    shape.Moment = cp.MomentForSegment(mass, vertex1, vertex2, 0.0f);
                }

            

            }
            //else if (null != fixtureValue["loop"])
            //{// support old
            //	// format (r197)

            //	JObject chainValue = (JObject)fixtureValue["loop"];
            //	b2ChainShape chainShape = new b2ChainShape();

            //	int numVertices = ((JArray)chainValue["x"]).Count;

            //	cpVect[] vertices = new cpVect[numVertices];
            //	for (int i = 0; i < numVertices; i++)
            //		vertices[i] = jsonToVec("vertices", chainValue, i);

            //	chainShape.CreateLoop(vertices, numVertices);

            //	fixtureDef.shape = chainShape;
            //	shape = body.CreateFixture(fixtureDef);

            //}
            else if (null != fixtureValue["chain"])
            {

                // FPE. See http://www.box2d.org/forum/viewtopic.php?f=4&t=7973&p=35363

                JObject chainValue = (JObject)fixtureValue["chain"];

                int numVertices = ((JArray)chainValue["vertices"]["x"]).Count;
                var vertices = new cpVect[numVertices];

                for (int i = 0; i < numVertices; i++)
                    vertices[i] = jsonToVec("vertices", chainValue, i);

                float radius = 0.2f;

                CCPhysicsShape shape;

                for (int i = 0; i < numVertices; i++)
                {
                    cpVect vertex1 = vertices[i];
                    cpVect vertex2 = vertices[(i + 1) % numVertices];

                    shape = new CCPhysicsShapeEdgeSegment(new CCPoint(vertex1.x, vertex1.y), new CCPoint(vertex2.x, vertex2.y), radius);//this function will end up only returning the last shape
                    shapes.Add(shape);
                    //SetBodyTypeFromInt(shape, type);

                    if (bodyType != cpBodyType.STATIC)
                    {       
                        float area = cp.AreaForSegment(vertex1, vertex2, radius);
                        float mass = density * area;
                        shape.Moment = cp.MomentForSegment(mass, vertex1, vertex2, 0.0f);//hmm. How to set correct moment without clobbering moment from existing shapes?
                    }
                }

            }
            else if (null != fixtureValue["polygon"])
            {

                JObject polygonValue = (JObject)fixtureValue["polygon"];
                cpVect[] vertices = new cpVect[b2_maxPolygonVertices];

                int numVertices = ((JArray)polygonValue["vertices"]["x"]).Count;


                int k = 0;
                for (int i = numVertices - 1; i >= 0; i--) // ohh... clockwise?!
                    vertices[k++] = jsonToVec("vertices", polygonValue, i);

                CCPoint[] points = new CCPoint[numVertices];
                for (int i = 0; i < numVertices; i++) // ohh... clockwise?!
                    points[i] = new CCPoint(vertices[i].x, vertices[i].y);


                CCPhysicsShape shape =new CCPhysicsShapePolygon(points, numVertices,CCPhysicsMaterial.PHYSICSSHAPE_MATERIAL_DEFAULT, 1);
                shapes.Add(shape);
                
                if (bodyType != cpBodyType.STATIC)
                {
                    float area = Math.Abs(cp.AreaForPoly(numVertices, vertices, 0.0f));
                    float mass = density * area;
                    shape.Moment = cp.MomentForPoly(mass, numVertices, vertices, cpVect.Zero, 0.0f);
                }

                //	}
                //else
                //{

                //	b2PolygonShape polygonShape = new b2PolygonShape();
                //	for (int i = 0; i < numVertices; i++)
                //		vertices[i] = jsonToVec("vertices", polygonValue, i);
                //	polygonShape.Set(vertices, numVertices);
                //	fixtureDef.shape = polygonShape;
                //	shape = body.CreateFixture(fixtureDef);
                //}
            }

            //String fixtureName = fixtureValue["name"] == null ? "" : fixtureValue["name"].ToString();
            //if (fixtureName != "")
            //{

            //    foreach (var shape in shape.GetShapes())
            //    {
            //        SetFixtureName(shape, fixtureName);
            //    }
            //}

            return shapes;
        }

        //CCPhysicsBody j2cpShape(JObject fixtureValue, int type)
        //{
        //    if (null == fixtureValue)
        //        return null;

        //    float restitution = jsonToFloat("restitution", fixtureValue);
        //    float friction = jsonToFloat("friction", fixtureValue);
        //    float density = jsonToFloat("density", fixtureValue);

        //    bool isSensor = fixtureValue["sensor"] == null ? false : (bool)fixtureValue["sensor"];

        //    ushort categoryBits = (fixtureValue["filter-categoryBits"] == null) ? (ushort)0x0001 : (ushort)fixtureValue["filter-categoryBits"];

        //    ushort maskBits = fixtureValue["filter-maskBits"] == null ? (ushort)0xffff : (ushort)fixtureValue["filter-maskBits"];

        //    short groupIndex = fixtureValue["filter-groupIndex"] == null ? (short)0 : (short)fixtureValue["filter-groupIndex"];

        //    CCPhysicsBody body = null;


        //    //CCPhysicsShape shape = null;

        //    if (null != fixtureValue["circle"])
        //    {
        //        JObject circleValue = (JObject)fixtureValue["circle"];

        //        CCPoint center = jsonToPoint("center", circleValue);

        //        float radius = jsonToFloat("radius", circleValue);

        //        body = CCPhysicsBody.CreateCircle(radius, center);
				
        //        //shape = new CCPhysicsShapeCircle(radius, center);
        //        float area = cp.AreaForCircle(0, radius);
        //        float mass = density * area;

        //        body.Moment = cp.MomentForCircle(mass, 0, radius, new cpVect(center.X, center.Y));

        //        SetBodyTypeFromInt(body,type);

        //    }
        //    else if (null != fixtureValue["edge"])
        //    {
					
        //        JObject edgeValue = (JObject)fixtureValue["edge"];
        //        cpVect vertex1 =jsonToVec("vertex1", edgeValue);
        //        cpVect vertex2 =jsonToVec("vertex1", edgeValue);
        //        float radius = 0;

        //        body = CCPhysicsBody.CreateEdgeSegment(new CCPoint(vertex1.x, vertex1.y), new CCPoint(vertex2.x, vertex2.y), radius);
			
        //        SetBodyTypeFromInt(body,type);

        //        if (body.BodyType != cpBodyType.STATIC)
        //        {

        //            float area = cp.AreaForSegment(vertex1, vertex2, radius);
        //            float mass = density * area;

        //            body.Moment = cp.MomentForSegment(mass, vertex1, vertex2, 0.0f);
        //        }

        //    }
        //    //else if (null != fixtureValue["loop"])
        //    //{// support old
        //    //	// format (r197)

        //    //	JObject chainValue = (JObject)fixtureValue["loop"];
        //    //	b2ChainShape chainShape = new b2ChainShape();

        //    //	int numVertices = ((JArray)chainValue["x"]).Count;

        //    //	cpVect[] vertices = new cpVect[numVertices];
        //    //	for (int i = 0; i < numVertices; i++)
        //    //		vertices[i] = jsonToVec("vertices", chainValue, i);

        //    //	chainShape.CreateLoop(vertices, numVertices);

        //    //	fixtureDef.shape = chainShape;
        //    //	shape = body.CreateFixture(fixtureDef);

        //    //}
        //    else if (null != fixtureValue["chain"])
        //    {

        //        // FPE. See http://www.box2d.org/forum/viewtopic.php?f=4&t=7973&p=35363

        //        JObject chainValue = (JObject)fixtureValue["chain"];
			
        //        int numVertices = ((JArray)chainValue["vertices"]["x"]).Count;
        //        var vertices = new cpVect[numVertices];

        //        for (int i = 0; i < numVertices; i++)
        //            vertices[i] = jsonToVec("vertices", chainValue, i);

        //        float radius = 0.1f;

        //        for (int i = 0; i < numVertices/2; i++)
        //        {
        //            cpVect vertex1 = vertices[i];
        //            cpVect vertex2 = vertices[(i + 1) % numVertices];

        //            body = CCPhysicsBody.CreateEdgeSegment(new CCPoint(vertex1.x, vertex1.y), new CCPoint(vertex2.x, vertex2.y), radius);//this function will end up only returning the last shape
				
        //            SetBodyTypeFromInt(body, type);

        //            if (body.BodyType != cpBodyType.STATIC)
        //            {
        //                float area = cp.AreaForSegment(vertex1, vertex2, radius);
        //                float mass = density * area;
        //                body.Moment = cp.MomentForSegment(mass, vertex1, vertex2, 0.0f);//hmm. How to set correct moment without clobbering moment from existing shapes?
        //            }
        //        }

        //    }
        //    else if (null != fixtureValue["polygon"])
        //    {

        //        JObject polygonValue = (JObject)fixtureValue["polygon"];
        //        cpVect[] vertices = new cpVect[b2_maxPolygonVertices];

        //        int numVertices = ((JArray)polygonValue["vertices"]["x"]).Count;


        //        int k = 0;
        //        for (int i = numVertices - 1; i >= 0; i--) // ohh... clockwise?!
        //            vertices[k++] = jsonToVec("vertices", polygonValue, i);

        //        CCPoint[] points = new CCPoint[numVertices];
        //        for (int i = 0; i < numVertices; i++) // ohh... clockwise?!
        //            points[i] = new CCPoint(vertices[i].x, vertices[i].y);


        //        body = CCPhysicsBody.CreatePolygon(points, numVertices, 1);
			
        //        SetBodyTypeFromInt(body, type);

        //        if (body.BodyType != cpBodyType.STATIC)
        //        {
        //            float area = Math.Abs(cp.AreaForPoly(numVertices, vertices, 0.0f));
        //            float mass = density * area;
        //            body.Moment = cp.MomentForPoly(mass, numVertices, vertices, cpVect.Zero, 0.0f);
        //        }

        //    //	}
        //        //else
        //        //{

        //        //	b2PolygonShape polygonShape = new b2PolygonShape();
        //        //	for (int i = 0; i < numVertices; i++)
        //        //		vertices[i] = jsonToVec("vertices", polygonValue, i);
        //        //	polygonShape.Set(vertices, numVertices);
        //        //	fixtureDef.shape = polygonShape;
        //        //	shape = body.CreateFixture(fixtureDef);
        //        //}
        //    }

        //    String fixtureName = fixtureValue["name"] == null ? "" : fixtureValue["name"].ToString();
        //    if (fixtureName != "")
        //    {

        //        foreach (var shape in body.GetShapes())
        //        {
        //            SetFixtureName(shape, fixtureName);
        //        }
        //    }

        //    return body;
        //}

		// SIN REVISAR

        CCPhysicsJoint j2cpConstraint(CCPhysicsWorld space, JObject jointValue)
		{
			CCPhysicsJoint joint = null;

			int bodyIndexA = (int)jointValue["bodyA"];
			int bodyIndexB = (int)jointValue["bodyB"];
			if (bodyIndexA >= m_bodies.Count || bodyIndexB >= m_bodies.Count)
				return null;

			// set features common to all joints
			//var bodyA = m_bodies[bodyIndexA];
			//var bodyB = m_bodies[bodyIndexB];
			//var collideConnected = jointValue["collideConnected"] == null ? false : (bool)jointValue["collideConnected"];

			// keep these in scope after the if/else below
			//b2RevoluteJointDef revoluteDef;
			//b2PrismaticJointDef prismaticDef;
			//b2DistanceJointDef distanceDef;
			//b2PulleyJointDef pulleyDef;
			//b2MouseJointDef mouseDef;
			//b2GearJointDef gearDef;
			//b2WheelJoint wheelDef;
			//b2WeldJointDef weldDef;
			//b2FrictionJointDef frictionDef;
			//b2RopeJointDef ropeDef;
			//MotorJoint motorDef;


			cpVect mouseJointTarget = new cpVect(0, 0);


			string type = jointValue["type"].ToString() == null ? "" : jointValue["type"].ToString();

			if (type == "revolute")
			{


				CCPoint localAnchorA = jsonToPoint("anchorA", jointValue);
				CCPoint localAnchorB = jsonToPoint("anchorB", jointValue);

//				joint = new cpPivotJoint(m_bodies[bodyIndexA], m_bodies[bodyIndexB], localAnchorA, localAnchorB);
	//			
				
				joint = CCPhysicsJointPin.Construct(m_bodies[bodyIndexA],m_bodies[bodyIndexB],localAnchorA);
                space.AddJoint(joint);
				
				//jointDef = revoluteDef = new b2RevoluteJointDef(); // JointFactory.CreateRevoluteJoint(world, bodyA, bodyB, jsonToVec("anchorB", jointValue));
				//revoluteDef.localAnchorA = jsonToVec("anchorA", jointValue);
				//revoluteDef.localAnchorB = jsonToVec("anchorB", jointValue);
				//revoluteDef.referenceAngle = jsonToFloat("refAngle", jointValue);
				//revoluteDef.enableLimit = jointValue["enableLimit"] == null ? false : (bool)jointValue["enableLimit"];
				//revoluteDef.lowerAngle = jsonToFloat("lowerLimit", jointValue);
				//revoluteDef.upperAngle = jsonToFloat("upperLimit", jointValue);
				//revoluteDef.enableMotor = jointValue["enableMotor"] == null ? false : (bool)jointValue["enableMotor"];
				//revoluteDef.motorSpeed = jsonToFloat("motorSpeed", jointValue);
				//revoluteDef.maxMotorTorque = jsonToFloat("maxMotorTorque", jointValue);
			}
			//else if (type == "prismatic")
			//{
				//jointDef = prismaticDef = new b2PrismaticJointDef(); //JointFactory.CreatePrismaticJoint(world, bodyA, bodyB, localAnchorB, localAxis);

				//prismaticDef.localAnchorA = jsonToVec("anchorA", jointValue);
				//prismaticDef.localAnchorB = jsonToVec("anchorB", jointValue);


				//if (jointValue["localAxisA"] != null)
				//	prismaticDef.localAxisA = jsonToVec("localAxisA", jointValue);
				//else
				//	prismaticDef.localAxisA = jsonToVec("localAxis1", jointValue);

				//prismaticDef.referenceAngle = jsonToFloat("refAngle", jointValue);

				//prismaticDef.enableLimit = jointValue["enableLimit"] == null ? false : (bool)jointValue["enableLimit"];

				//prismaticDef.lowerTranslation = jsonToFloat("lowerLimit", jointValue);
				//prismaticDef.upperTranslation = jsonToFloat("upperLimit", jointValue);

				//prismaticDef.enableMotor = jointValue["enableMotor"] == null ? false : (bool)jointValue["enableMotor"];

				//prismaticDef.motorSpeed = jsonToFloat("motorSpeed", jointValue);
				//prismaticDef.maxMotorForce = jsonToFloat("maxMotorForce", jointValue);

			//}
			else if (type == "distance")
			{


				CCPoint localAnchorA = jsonToPoint("anchorA", jointValue);
				CCPoint localAnchorB = jsonToPoint("anchorB", jointValue);
				float length = jsonToFloat("length", jointValue);
				float stiffness = jsonToFloat("frequency", jointValue);
				float damping = jsonToFloat("dampingRatio", jointValue);

				joint = CCPhysicsJointSpring.Construct(m_bodies[bodyIndexA], m_bodies[bodyIndexB], localAnchorA, localAnchorB, stiffness, damping);
				
			//	joint =new cpDampedSpring(m_bodies[bodyIndexA], m_bodies[bodyIndexB], localAnchorA, localAnchorB, length, stiffness, damping);
                space.AddJoint(joint);

				//jointDef = distanceDef = new b2DistanceJointDef();
				//distanceDef.localAnchorA = jsonToVec("anchorA", jointValue);
				//distanceDef.localAnchorB = jsonToVec("anchorB", jointValue);
				//distanceDef.length = jsonToFloat("length", jointValue);
				//distanceDef.frequencyHz = jsonToFloat("frequency", jointValue);
				//distanceDef.dampingRatio = jsonToFloat("dampingRatio", jointValue);

			}
			//else if (type == "pulley")
			//{

			//	jointDef = pulleyDef = new b2PulleyJointDef();
			//	pulleyDef.groundAnchorA = jsonToVec("groundAnchorA", jointValue);
			//	pulleyDef.groundAnchorB = jsonToVec("groundAnchorB", jointValue);
			//	pulleyDef.localAnchorA = jsonToVec("anchorA", jointValue);
			//	pulleyDef.localAnchorB = jsonToVec("anchorB", jointValue);
			//	pulleyDef.lengthA = jsonToFloat("lengthA", jointValue);
			//	pulleyDef.lengthB = jsonToFloat("lengthB", jointValue);
			//	pulleyDef.ratio = jsonToFloat("ratio", jointValue);

			//}
			//else if (type == "mouse")
			//{
			//	jointDef = mouseDef = new b2MouseJointDef();
			//	mouseJointTarget = jsonToVec("target", jointValue);
			//	mouseDef.target = jsonToVec("anchorB", jointValue);// alter after creating joint
			//	mouseDef.maxForce = jsonToFloat("maxForce", jointValue);
			//	mouseDef.frequencyHz = jsonToFloat("frequency", jointValue);
			//	mouseDef.dampingRatio = jsonToFloat("dampingRatio", jointValue);
			//}
			// Gear joints are apparently not implemented in JBox2D yet, but
			// when they are, commenting out the following section should work.

			//else if (type == "gear")
			//{

			//	jointDef = gearDef = new b2GearJointDef();  //JointFactory.CreateGearJoint(world, joint1, joint2, ratio);
			//	int jointIndex1 = (int)jointValue["joint1"];
			//	int jointIndex2 = (int)jointValue["joint2"];
			//	var joint1 = m_joints[jointIndex1];
			//	var joint2 = m_joints[jointIndex2];
			//	var ratio = jsonToFloat("ratio", jointValue);

			//	//joint = gearDef = JointFactory.CreateGearJoint(world, joint1, joint2, ratio);

			//}

			// Wheel joints are apparently not implemented in JBox2D yet, but
			// when they are, commenting out the following section should work.

			else if (type == "wheel")
			{

				CCPoint localAnchorA = jsonToPoint("anchorA", jointValue);
                CCPoint localAnchorB = jsonToPoint("anchorB", jointValue);
                CCPoint localAxisA = jsonToPoint("localAxisA", jointValue);
				//joint =new cpGrooveJoint(m_bodies[bodyIndexA], m_bodies[bodyIndexB], localAnchorA, cpVect.cpvadd(localAnchorA, localAxisA), localAnchorB);
                joint =CCPhysicsJointGroove.Construct(m_bodies[bodyIndexA], m_bodies[bodyIndexB], localAnchorA, localAnchorA+localAxisA, localAnchorB);
                space.AddJoint(joint);

				//also add a distance joint
				//Chipmunk groove joints have limits whereas b2WheelJoints do not, and chipmunk damped springs
				//seem to be only one way (ie. only preventing compression) whereas b2WheelJoints are two-way,
				//for lack of a better description. This can be accounted for in a rather messy way by using
				//the length of the axis to adjust the target position of the spring so that it is not right
				//on top of the limit of the groove joint.
				float length = localAxisA.Length * 0.5f;
				float stiffness = jsonToFloat("springFrequency", jointValue);
				float damping = jsonToFloat("springDampingRatio", jointValue);

				joint =CCPhysicsJointSpring.Construct(m_bodies[bodyIndexA], m_bodies[bodyIndexB], localAnchorA, localAnchorB, stiffness, damping);
				space.AddJoint( joint);

				//jointDef = revoluteDef = new b2RevoluteJointDef();
				//revoluteDef.localAnchorA = jsonToVec("anchorA", jointValue);
				//revoluteDef.localAnchorB = jsonToVec("anchorB", jointValue);

				//revoluteDef.enableMotor = jointValue["enableMotor"] == null ? false : (bool)jointValue["enableMotor"];

				//revoluteDef.motorSpeed = jsonToFloat("motorSpeed", jointValue);
				//revoluteDef.maxMotorTorque = jsonToFloat("maxMotorTorque", jointValue);

				//jointDef = wheelDef = new b2WheelJointDef(); //JointFactory.CreateWheelJoint(world, bodyA, bodyB, localAnchorB, localAxisA);


				//var localAnchorA = jsonToVec("anchorA", jointValue);
				//var localAnchorB = (jsonToVec("anchorB", jointValue));
				//var localAxisA = (jsonToVec("localAxisA", jointValue));
				//var enableMotor = jointValue["enableMotor"] == null ? false : (bool)jointValue["enableMotor"];
				//var motorSpeed = jsonToFloat("motorSpeed", jointValue);
				//var maxMotorTorque = jsonToFloat("maxMotorTorque", jointValue);
				//var frequencyHz = jsonToFloat("springFrequency", jointValue);
				//var dampingRatio = jsonToFloat("springDampingRatio", jointValue);

				//wheelDef.LocalAnchorA = localAnchorA;
				//wheelDef.LocalAnchorB = localAnchorB;
				//wheelDef.MotorEnabled = enableMotor;
				//wheelDef.MotorSpeed = motorSpeed;
				//wheelDef.SpringFrequencyHz = frequencyHz;
				//wheelDef.MaxMotorTorque = maxMotorTorque;
				//wheelDef.SpringDampingRatio = dampingRatio;
			}
			//else if (type == "weld")
			//{
			//	jointDef = weldDef = new b2WeldJointDef();
			//	weldDef.localAnchorA = jsonToVec("anchorA", jointValue);
			//	weldDef.localAnchorB = jsonToVec("anchorB", jointValue);
			//	weldDef.referenceAngle = 0;

			//}
			//else if (type == "friction")
			//{
			//	jointDef = frictionDef = new b2FrictionJointDef();
			//	frictionDef.localAnchorA = jsonToVec("anchorA", jointValue);
			//	frictionDef.localAnchorB = jsonToVec("anchorB", jointValue);
			//	frictionDef.maxForce = jsonToFloat("maxForce", jointValue);
			//	frictionDef.maxTorque = jsonToFloat("maxTorque", jointValue);
			//}
			//else if (type == "rope")
			//{
			//	jointDef = ropeDef = new b2RopeJointDef();
			//	ropeDef.localAnchorA = jsonToVec("anchorA", jointValue);
			//	ropeDef.localAnchorB = jsonToVec("anchorB", jointValue);
			//	ropeDef.maxLength = jsonToFloat("maxLength", jointValue);
			//}

			//else if (type == "motor")
			//{
			//    var maxForce = jsonToFloat("maxForce", jointValue);
			//    var maxMotorTorque = jsonToFloat("maxTorque", jointValue);
			//    var angularOffset = jsonToFloat("refAngle", jointValue);

			//    joint = motorDef = new MotorJoint(bodyA, bodyB);
			//    world.AddJoint(joint);
			//    motorDef.LinearOffset = jsonToVec("anchorA", jointValue);
			//    motorDef.MaxForce = maxForce;
			//    motorDef.MaxTorque = maxMotorTorque;
			//    motorDef.AngularOffset = angularOffset;
			//}


			//if (null != jointDef)
			//{
			//	// set features common to all joints
			//	jointDef.BodyA = m_bodies[bodyIndexA];
			//	jointDef.BodyB = m_bodies[bodyIndexB];

			//	jointDef.CollideConnected = jointValue["collideConnected"] == null ? false : (bool)jointValue["collideConnected"];

			//	joint = space.CreateJoint(jointDef);

			//	if (type.Equals("mouse"))
			//		((b2MouseJoint)joint).SetTarget(mouseJointTarget);

			//	String jointName = jointValue["name"] == null ? "" : (string)jointValue["name"];

			//	if (!jointName.Equals(""))
			//	{
			//		SetJointName(joint, jointName);
			//	}
			//}


			return joint;
		}

        b2dJsonImage j2b2dJsonImage(JObject imageValue)
		{
			b2dJsonImage img = new b2dJsonImage();

			int bodyIndex = imageValue["body"] == null ? -1 : (int)imageValue["body"];
			if (-1 != bodyIndex)
				img.Body = lookupBodyFromIndex(bodyIndex);

			String imageName = imageValue["name"] == null ? "" : imageValue["name"].ToString();
			if (imageName != "")
			{
				img.Name = imageName;
				SetImageName(img, imageName);
			}

			String fileName = imageValue["file"] == null ? "" : imageValue["file"].ToString();
			//TODO: Renombramos el fichero a una ruta relativa
			if (fileName != "")
				img.File = Path.GetFileName(fileName);

			img.Center = jsonToVec("center", imageValue);
			img.Angle = jsonToFloat("angle", imageValue);
			img.Scale = jsonToFloat("scale", imageValue);
			img.Opacity = jsonToFloat("opacity", imageValue);
			img.RenderOrder = jsonToFloat("renderOrder", imageValue);

			JArray colorTintArray = (JArray)imageValue["colorTint"];
			if (null != colorTintArray)
			{
				for (int i = 0; i < 4; i++)
				{
					img.ColorTint[i] = (int)colorTintArray[i];
				}
			}

			img.Flip = imageValue["flip"] == null ? false : (bool)imageValue["flip"];

			img.Filter = imageValue["filter"] == null ? (_b2dJsonImagefilterType) 1 : (_b2dJsonImagefilterType)(int)imageValue["filter"];

			img.m_corners = new cpVect[4];
			for (int i = 0; i < 4; i++)
				img.m_corners[i] = jsonToVec("corners", imageValue, i);

			JArray vertexPointerArray = (JArray)imageValue["glVertexPointer"];
			JArray texCoordArray = (JArray)imageValue["glVertexPointer"];
			if (null != vertexPointerArray && null != texCoordArray && vertexPointerArray.Count == texCoordArray.Count)
			{
				int numFloats = vertexPointerArray.Count;
				img.NumPoints = numFloats / 2;
				img.Points = new float[numFloats];
				img.UvCoords = new float[numFloats];
				for (int i = 0; i < numFloats; i++)
				{
					img.Points[i] = jsonToFloat("glVertexPointer", imageValue, i);
					img.UvCoords[i] = jsonToFloat("glTexCoordPointer", imageValue, i);
				}
			}

			JArray drawElementsArray = (JArray)imageValue["glDrawElements"];
			if (null != drawElementsArray)
			{
				img.NumIndices = drawElementsArray.Count;
				img.Indices = new short[img.NumIndices];
				for (int i = 0; i < img.NumIndices; i++)
					img.Indices[i] = (short)drawElementsArray[i];
			}

			return img;
		}





        protected void readCustomPropertiesFromJson(CCPhysicsBody item, JObject value)
		{
			if (null == item)
				return;

			if (value["customProperties"] != null)
				return;

			int i = 0;
			JArray propValues = (JArray)value["customProperties"];
			if (null != propValues)
			{
				int numPropValues = propValues.Count;
				for (i = 0; i < numPropValues; i++)
				{
					JObject propValue = (JObject)propValues[i];
					string propertyName = propValue["name"].ToString();
					if (propValue["int"] != null)
						SetCustomInt(item, propertyName, (int)propValue["int"]);
					if (propValue["float"] != null)
						SetCustomFloat(item, propertyName, (float)propValue["float"]);
					if (propValue["string"] != null)
						SetCustomString(item, propertyName, propValue["string"].ToString());
					if (propValue["vec2"] != null)
						SetCustomVector(item, propertyName, this.jsonToVec("vec2", propValue));
					if (propValue["bool"] != null)
						SetCustomBool(item, propertyName, (bool)propValue["bool"]);
				}
			}
		}
        protected void readCustomPropertiesFromJson(List<CCPhysicsShape> shapes, JObject value)
        {
            foreach (var item in shapes)
            {
                readCustomPropertiesFromJson(item, value);
            }
        }

		protected void readCustomPropertiesFromJson(CCPhysicsShape item, JObject value)
		{
			if (null == item)
				return;

			if (value["customProperties"] != null)
				return;

			int i = 0;
			JArray propValues = (JArray)value["customProperties"];
			if (null != propValues)
			{
				int numPropValues = propValues.Count;
				for (i = 0; i < numPropValues; i++)
				{
					JObject propValue = (JObject)propValues[i];
					String propertyName = propValue["name"].ToString();
					if (propValue["int"] != null)
						SetCustomInt(item, propertyName, (int)propValue["int"]);
					if (propValue["float"] != null)
						SetCustomFloat(item, propertyName, (float)propValue["float"]);
					if (propValue["string"] != null)
						SetCustomString(item, propertyName, propValue["string"].ToString());
					if (propValue["vec2"] != null)
						SetCustomVector(item, propertyName, this.jsonToVec("vec2", propValue));
					if (propValue["bool"] != null)
						SetCustomBool(item, propertyName, (bool)propValue["bool"]);
				}
			}
		}

		protected void readCustomPropertiesFromJson(CCPhysicsJoint item, JObject value)
		{
			if (null == item)
				return;

			if (value["customProperties"] != null)
				return;

			int i = 0;
			JArray propValues = (JArray)value["customProperties"];
			if (null != propValues)
			{
				int numPropValues = propValues.Count;
				for (i = 0; i < numPropValues; i++)
				{
					JObject propValue = (JObject)propValues[i];
					String propertyName = propValue["name"].ToString();
					if (propValue["int"] != null)
						SetCustomInt(item, propertyName, (int)propValue["int"]);
					if (propValue["float"] != null)
						SetCustomFloat(item, propertyName, (float)propValue["float"]);
					if (propValue["string"] != null)
						SetCustomString(item, propertyName, propValue["string"].ToString());
					if (propValue["vec2"] != null)
						SetCustomVector(item, propertyName, this.jsonToVec("vec2", propValue));
					if (propValue["bool"] != null)
						SetCustomBool(item, propertyName, (bool)propValue["bool"]);
				}
			}
		}

		protected void readCustomPropertiesFromJson(b2dJsonImage item, JObject value)
		{
			if (null == item)
				return;

			if (value["customProperties"] != null)
				return;

			int i = 0;
			JArray propValues = (JArray)value["customProperties"];
			if (null != propValues)
			{
				int numPropValues = propValues.Count;
				for (i = 0; i < numPropValues; i++)
				{
					JObject propValue = (JObject)propValues[i];
					string propertyName = propValue["name"].ToString();
					if (propValue["int"] != null)
						SetCustomInt(item, propertyName, (int)propValue["int"]);
					if (propValue["float"] != null)
						SetCustomFloat(item, propertyName, (float)propValue["float"]);
					if (propValue["string"] != null)
						SetCustomString(item, propertyName, propValue["string"].ToString());
					if (propValue["vec2"] != null)
						SetCustomVector(item, propertyName, this.jsonToVec("vec2", propValue));
					if (propValue["bool"] != null)
						SetCustomBool(item, propertyName, (bool)propValue["bool"]);
				}
			}
		}

		protected void readCustomPropertiesFromJson(CCPhysicsWorld item, JObject value)
		{
			if (null == item)
				return;

			if (value["customProperties"] != null)
				return;

			int i = 0;
			JArray propValues = (JArray)value["customProperties"];
			if (null != propValues)
			{
				int numPropValues = propValues.Count;
				for (i = 0; i < numPropValues; i++)
				{
					JObject propValue = (JObject)propValues[i];
					String propertyName = propValue["name"].ToString();
					if (propValue["int"] != null)
						SetCustomInt(item, propertyName, (int)propValue["int"]);
					if (propValue["float"] != null)
						SetCustomFloat(item, propertyName, (float)propValue["float"]);
					if (propValue["string"] != null)
						SetCustomString(item, propertyName, propValue["string"].ToString());
					if (propValue["vec2"] != null)
						SetCustomVector(item, propertyName, this.jsonToVec("vec2", propValue));
					if (propValue["bool"] != null)
						SetCustomBool(item, propertyName, (bool)propValue["bool"]);
				}
			}
		}




		// setCustomXXX

		protected void SetCustomInt(Object item, String propertyName, int val)
		{
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_int.Add(propertyName, val);
		}

		protected void SetCustomFloat(Object item, String propertyName, float val)
		{
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_float.Add(propertyName, (float)val);
		}

		protected void SetCustomString(Object item, String propertyName, String val)
		{
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_string.Add(propertyName, val);
		}

		protected void SetCustomVector(Object item, String propertyName, cpVect val)
		{
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_cpVect.Add(propertyName, val);
		}

		protected void SetCustomBool(Object item, String propertyName, bool val)
		{
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_bool.Add(propertyName, val);
		}



		public void SetCustomInt(CCPhysicsJoint item, String propertyName, int val)
		{
			m_jointsWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_int.Add(propertyName, val);
		}

		public void SetCustomFloat(CCPhysicsJoint item, String propertyName, float val)
		{
			m_jointsWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_float.Add(propertyName, (float)val);
		}

		public void SetCustomString(CCPhysicsJoint item, String propertyName, String val)
		{
			m_jointsWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_string.Add(propertyName, val);
		}

		public void SetCustomVector(CCPhysicsJoint item, String propertyName, cpVect val)
		{
			m_jointsWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_cpVect.Add(propertyName, val);
		}

		public void SetCustomBool(CCPhysicsJoint item, String propertyName, bool val)
		{
			m_jointsWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_bool.Add(propertyName, val);
		}


		public void SetCustomInt(b2dJsonImage item, String propertyName, int val)
		{
			m_imagesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_int.Add(propertyName, val);
		}

		public void SetCustomFloat(b2dJsonImage item, String propertyName, float val)
		{
			m_imagesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_float.Add(propertyName, (float)val);
		}

		public void SetCustomString(b2dJsonImage item, String propertyName, String val)
		{
			m_imagesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_string.Add(propertyName, val);
		}

		public void SetCustomVector(b2dJsonImage item, String propertyName, cpVect val)
		{
			m_imagesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_cpVect.Add(propertyName, val);
		}

		public void SetCustomBool(b2dJsonImage item, String propertyName, bool val)
		{
			m_imagesWithCustomProperties.Add(item);
			GetCustomPropertiesForItem(item, true).m_customPropertyMap_bool.Add(propertyName, val);
		}

		CCPoint jsonToPoint(String name, JObject value)
		{
			return jsonToVec(name, value, -1, new CCPoint(0, 0));
		}

		cpVect jsonToVec(String name, JObject value)
		{
			return jsonToVec(name, value, -1, new cpVect(0, 0));
		}

		cpVect jsonToVec(String name, JObject value, int index)
		{
			return jsonToVec(name, value, index, new cpVect(0, 0));
		}

		CCPoint jsonToVec(String name, JObject value, int index, CCPoint defaultValue)
		{
			CCPoint vec = defaultValue;

			if (value[name] == null || value[name] is JValue)
				return defaultValue;

			if (index > -1)
			{
				JObject vecValue = (JObject)value[name];
				JArray arrayX = (JArray)vecValue["x"];
				JArray arrayY = (JArray)vecValue["y"];

				vec.X = (float)arrayX[index];

				vec.Y = (float)arrayY[index];
			}
			else
			{
				JObject vecValue = (JObject)value[name];
				if (null == vecValue)
					return defaultValue;
				else if (vecValue["x"] == null) // should be zero vector
					vec.X = vec.Y = 0;
				else
				{
					vec.X = jsonToFloat("x", vecValue);
					vec.Y = jsonToFloat("y", vecValue);
				}
			}

			return vec;
		}

		cpVect jsonToVec(String name, JObject value, int index, cpVect defaultValue)
		{
			cpVect vec = defaultValue;

			if (value[name] == null || value[name] is JValue)
				return defaultValue;

			if (index > -1)
			{
				JObject vecValue = (JObject)value[name];
				JArray arrayX = (JArray)vecValue["x"];
				JArray arrayY = (JArray)vecValue["y"];

				vec.x = (float)arrayX[index];

				vec.y = (float)arrayY[index];
			}
			else
			{
				JObject vecValue = (JObject)value[name];
				if (null == vecValue)
					return defaultValue;
				else if (vecValue["x"] == null) // should be zero vector
					vec.x = vec.y = 0;
				else
				{
					vec.x = jsonToFloat("x", vecValue);
					vec.y = jsonToFloat("y", vecValue);
				}
			}

			return vec;
		}

		float jsonToFloat(String name, JObject value)
		{
			return jsonToFloat(name, value, -1, 0);
		}

		float jsonToFloat(String name, JObject value, int index)
		{
			return jsonToFloat(name, value, index, 0);
		}

		float jsonToFloat(String name, JObject value, int index, float defaultValue)
		{
			if (value[name] == null)
				return defaultValue;

			if (index > -1)
			{
				JArray array = null;
				try
				{
					array = (JArray)value[name];
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				if (null == array)
					return defaultValue;
				Object obj = array[index];
				if (null == obj)
					return defaultValue;
				// else if ( value[name].isString() )
				// return hexToFloat( value[name].asString() );
				else
					return float.Parse(obj.ToString());
			}
			else
			{
				Object obj = value[name];
				if (null == obj)
					return defaultValue;
				// else if ( value[name].isString() )
				// return hexToFloat( value[name].asString() );
				else
					return float.Parse(obj.ToString());
			}
		}

		#endregion


		public int b2_maxPolygonVertices = 10000;
	}

}


