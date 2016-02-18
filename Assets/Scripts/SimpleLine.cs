using UnityEngine;
using System.Collections.Generic;


public class SimpleLine : MonoBehaviour {

	public Shader shader;

	private Mesh ml;
	private Material lmat;
	
	private Mesh ms;
	private Material smat;
	
	private Vector3 s;

	private float lineSize = 0.03f;
	
	private GUIStyle labelStyle;
	private GUIStyle linkStyle;
	

	void Start () {
		ml = new Mesh();
		lmat = new Material(shader);
		lmat.color = new Color(0,0,0,0.3f);
		
		Vector3 a = new Vector3 (1F, 2F, 0F);
		Vector3 b = new Vector3 (6F, 5F, 3F);


		AddLine(ml, MakeQuad(a, b, lineSize), false);

		GameObject sphere1 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere1.transform.position = a;
		sphere1.transform.localScale = new Vector3 (1F, 1F, 1F);
		
		GameObject sphere2 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		sphere2.transform.position = b;
		sphere2.transform.localScale = new Vector3 (1F, 1F, 1F);
	}

	void Update() {

		Graphics.DrawMesh(ml, transform.localToWorldMatrix, lmat, 0);
	}
	
	Vector3[] MakeQuad(Vector3 s, Vector3 e, float w) {
		w = w / 2;
		Vector3[] q = new Vector3[4];

		Vector3 n = Vector3.Cross(s, e);
		Vector3 l = Vector3.Cross(n, e-s);
		l.Normalize();
		
		q[0] = transform.InverseTransformPoint(s + l * w);
		q[1] = transform.InverseTransformPoint(s + l * -w);
		q[2] = transform.InverseTransformPoint(e + l * w);
		q[3] = transform.InverseTransformPoint(e + l * -w);

		return q;
	}
	
	void AddLine(Mesh m, Vector3[] quad, bool tmp) {
			int vl = m.vertices.Length;
			
			Vector3[] vs = m.vertices;
			if(!tmp || vl == 0) vs = resizeVertices(vs, 4);
			else vl -= 4;
			
			vs[vl] = quad[0];
			vs[vl+1] = quad[1];
			vs[vl+2] = quad[2];
			vs[vl+3] = quad[3];
			
			int tl = m.triangles.Length;
			
			int[] ts = m.triangles;
			if(!tmp || tl == 0) ts = resizeTraingles(ts, 6);
			else tl -= 6;
			ts[tl] = vl;
			ts[tl+1] = vl+1;
			ts[tl+2] = vl+2;
			ts[tl+3] = vl+1;
			ts[tl+4] = vl+3;
			ts[tl+5] = vl+2;
			
			m.vertices = vs;
			m.triangles = ts;
			m.RecalculateBounds();
	}
	
	Vector3[] resizeVertices(Vector3[] ovs, int ns) {
		Vector3[] nvs = new Vector3[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
	
	int[] resizeTraingles(int[] ovs, int ns) {
		int[] nvs = new int[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
	
}
	







