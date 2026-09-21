import bpy
import math
import os

#blender --background --python make_wave.py
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
OUTPUT_PATH = os.path.join(SCRIPT_DIR, "wave.glb")

WAVE_HEIGHT = 0.035
WAVE_WIDTH = 0.025
WAVE_LENGTH = 0.38

bpy.ops.object.select_all(action="SELECT")
bpy.ops.object.delete(use_global=False)

mat = bpy.data.materials.new("WaveMaterial")
mat.diffuse_color = (0.72, 0.90, 1.0, 1.0)
mat.metallic = 0.0
mat.roughness = 0.65

def add_wave(name, cx, cy, rotation, length, width, height):
    mesh = bpy.data.meshes.new(name + "Mesh")
    segments = 8
    verts, faces = [], []

    for i in range(segments + 1):
        t = i / segments
        x = (t - 0.5) * length
        y = math.sin((t - 0.5) * math.pi) * width * 0.7
        z = math.sin(t * math.pi) * height
        half_w = width * (0.45 + 0.55 * math.sin(t * math.pi))
        verts += [(x, y-half_w, z), (x, y+half_w, z)]

    for i in range(segments):
        a = i * 2
        faces.append((a, a+2, a+3, a+1))

    mesh.from_pydata(verts, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(obj)
    obj.data.materials.append(mat)
    obj.location = (cx, cy, 0.012)
    obj.rotation_euler[2] = rotation

waves = [
    (-0.12, 0.10, math.radians(8), WAVE_LENGTH*0.75, WAVE_WIDTH, WAVE_HEIGHT*0.75),
    (0.08, 0.00, math.radians(-7), WAVE_LENGTH, WAVE_WIDTH, WAVE_HEIGHT),
    (-0.03, -0.12, math.radians(5), WAVE_LENGTH*0.60, WAVE_WIDTH*0.9, WAVE_HEIGHT*0.65),
]

for i, args in enumerate(waves):
    add_wave(f"Wave_{i+1}", *args)

bpy.ops.object.select_all(action="DESELECT")
wave_objs = [o for o in bpy.context.collection.objects if o.name.startswith("Wave_")]
for obj in wave_objs:
    obj.select_set(True)
bpy.context.view_layer.objects.active = wave_objs[0]
bpy.ops.object.join()
bpy.context.active_object.name = "Wave"

bpy.ops.export_scene.gltf(filepath=OUTPUT_PATH, export_format="GLB")
print("Created:", OUTPUT_PATH)
