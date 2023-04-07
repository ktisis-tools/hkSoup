### This is a proof-of-concept WIP, not a completed tool!!

import os

import bpy
from bpy_extras.io_utils import ExportHelper

import xml.etree.ElementTree as ET

## Bones

def get_all_bones():
    bones = []
    # TODO: this takes a while to update after renaming bones, etc
    armature = next((o.data for o in bpy.data.objects if o.name in bpy.data.armatures))
    for bone in armature.bones:
        bones.append(bone)
    print("bones ct " + str(len(bones)))
    return bones

## XML

def read_orig_bones(skele):
    names = filter(lambda e : e.get("name") == "name", skele.findall("array/struct/string"))
    return list(map(lambda e : e.text, names))

def write_xml(base):
    root = base.getroot()
    skeles = list(filter(lambda e : e.get("type") == "hkaSkeleton", root.iterfind("object")))
    
    for skele in skeles:
        skele_bones = read_orig_bones(skele)
        bones_new = list(filter(lambda bone : bone.name not in skele_bones, get_all_bones()))
        all_bone_names = skele_bones + bones_new
        new_size = str(len(all_bone_names))
        
        arrays = skele.iterfind("array")
        
        pIndices = next((e for e in arrays if e.get("name") == "parentIndices"))
        pIndices.set("size", new_size)
        
        bones = next((e for e in arrays if e.get("name") == "bones"))
        bones.set("size", new_size)
        
        refs = next((e for e in arrays if e.get("name") == "referencePose"))
        refs.set("size", new_size)
        
        for bone in bones_new:
            px = -1 if bone.parent == None else all_bone_names.index(bone.parent.name)
            pIndices.text = pIndices.text + str(px) + " "
            
            name_struct = ET.SubElement(bones, "struct", {})
            name_string = ET.SubElement(name_struct, "string", {"name":"name"})
            name_string.text = bone.name
            print(name_string.get("name", "name"))
            
            ref_vec = ET.SubElement(refs, "vec12", {})
            ref_vec.text = ("x00000000 " * 3) + "x3f800000 " + ("x00000000 " * 3) + "x3f800000 " + ("x3f800000 " * 4)

## Export operator

class SklbExportOperator(bpy.types.Operator, ExportHelper):
    bl_idname = "hksoup.export"
    bl_label = "Confirm"
    
    filename_ext = ".xml"

    filepath: bpy.props.StringProperty(subtype="FILE_PATH")
    filter_glob: bpy.props.StringProperty(
        default='*.xml',
        options={'HIDDEN'}
    )
    
    title: bpy.props.StringProperty(default="aaa")
    
    @classmethod
    def poll(cls, ctx):
        return True # TODO
    
    def execute(self, ctx):
        _name, ext = os.path.splitext(self.filepath)
        if ext != ".xml":
            self.report({"ERROR"}, "Base skeleton must be an XML file.")
        
        if self.flow == 0:
            self.base = ET.parse(self.filepath)
            write_xml(self.base)
            self.flow = 1
            ctx.window_manager.fileselect_add(self)
            return {"RUNNING_MODAL"}
        elif self.flow == 1:
            self.base.write(self.filepath)
            
        return {"FINISHED"}
    
    def invoke(self, ctx, event):
        self.flow = 0
        ctx.window_manager.fileselect_add(self)
        return {"RUNNING_MODAL"}

def menu_func(self, ctx):
    self.layout.operator_context = "INVOKE_DEFAULT"
    self.layout.operator(SklbExportOperator.bl_idname, text="hkSoup Skeleton (.xml)")

## Plugin registration

bl_info = {
    "name": "hkSoup",
    "author": "Chirp",
    "description": "skellybones",
    "blender": (3, 5, 0)
}

def register_classes():
    bpy.utils.register_class(SklbExportOperator)

def register():
    print("__register__")
    register_classes()
    bpy.types.TOPBAR_MT_file_export.append(menu_func)

def unregister():
    print("__unregister__")
    bpy.types.TOPBAR_MT_file_export.remove(menu_func)

if __name__ == "__main__":
    #register()
    register_classes()
    bpy.ops.hksoup.export('INVOKE_DEFAULT')