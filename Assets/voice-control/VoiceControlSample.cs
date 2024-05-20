/* 
*   VideoKit
*   Copyright (c) 2024 NatML Inc. All Rights Reserved.
*/

namespace VideoKit.Examples {

    using System.ComponentModel;
    using System.Runtime.Serialization;
    using UnityEngine;

    public sealed class VoiceControlSample : MonoBehaviour {

        public MeshRenderer cube;

        private enum CubeColor {
            [EnumMember(Value = "red")]
            Red = 0,
            [EnumMember(Value = "green")]
            Green = 1,
            [EnumMember(Value = "blue")]
            Blue = 2,
        }

        private struct VoiceCommand {
            [Description(@"The cube color.")]
            public CubeColor color;
        }

        public async void OnVoiceCommand (MediaAsset asset) {
            // Caption the audio
            var command = await asset.Caption<VoiceCommand>();
            // Color the cube
            cube.material.color = ToColor(command.color);
        }

        private static Color ToColor (CubeColor color) => color switch {
            CubeColor.Red   => Color.red,
            CubeColor.Green => Color.green,
            CubeColor.Blue  => Color.blue,
            _               => Color.black,
        };
    }
}