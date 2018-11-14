import React, { Component } from 'react';
import {
  ActivityIndicator,
  Image,
  StyleSheet,
  ImageStore
} from 'react-native';
import { Button, Content, Icon, Segment } from 'native-base';
import { Constants, ImagePicker, Permissions } from 'expo';


export default class CameraPage extends Component {
  state = {
    image: null,
    uploading: false,
  };

  constructor(props) {
    super(props);
  }

  render() {
    let {
      image
    } = this.state;

    return (
      <Content style={styles.container} alignItems='center' justifyContent='center'>
        <Segment style={{ marginTop: 10 }}>
          <Button
            style={styles.primaryButton}
            onPress={this._pickImage.bind(this)}
          >
            <Icon name="ios-folder" style={{ margin: 5, color: 'white' }} />
          </Button>
          <Button
            style={styles.primaryButton}
            onPress={this._takePhoto.bind(this)}>
            <Icon name="ios-camera" style={{ margin: 5, color: 'white' }} />
          </Button>
        </Segment>

        {this._maybeRenderImage()}
        {this._maybeRenderUploadingOverlay()}
      </Content>
    );
  }

  _maybeRenderUploadingOverlay = () => {
    if (this.state.uploading) {
      return (
        <Content
          style={[StyleSheet.absoluteFill, styles.maybeRenderUploading]}
          justifyContent='center'
          alignItems='center'>
          <ActivityIndicator color="#fff" size="large" />
        </Content>
      );
    }
  };

  _maybeRenderImage = () => {
    let {
      image
    } = this.state;

    if (!image) {
      return;
    }

    return (
      <Content
        style={styles.maybeRenderContainer}>
        <Content
          style={styles.maybeRenderImageContainer}>
          <Image source={{ uri: image.uri }} style={styles.maybeRenderImage} />
        </Content>
      </Content>
    );
  };

  _takePhoto = async () => {
    const {
      status: cameraPerm
    } = await Permissions.askAsync(Permissions.CAMERA);

    const {
      status: cameraRollPerm
    } = await Permissions.askAsync(Permissions.CAMERA_ROLL);

    // only if user allows permission to camera AND camera roll
    if (cameraPerm === 'granted' && cameraRollPerm === 'granted') {
      let pickerResult = await ImagePicker.launchCameraAsync({
        allowsEditing: true,
        aspect: [210, 297],
        base64: true,
        quality: 0.7
      });

      this._handleImagePicked(pickerResult);
    }
  };

  _pickImage = async () => {
    const {
      status: cameraRollPerm
    } = await Permissions.askAsync(Permissions.CAMERA_ROLL);

    // only if user allows permission to camera roll
    if (cameraRollPerm === 'granted') {
      let pickerResult = await ImagePicker.launchImageLibraryAsync({
        allowsEditing: true,
        aspect: [210, 297],
        base64: true,
        quality: 0.7
      });

      this._handleImagePicked(pickerResult);
    }
  };

  _handleImagePicked = async pickerResult => {
    let uploadResponse, uploadResult;

    try {
      this.setState({
        uploading: true
      });

      if (!pickerResult.cancelled) {
        let self = this;
        if (Constants.platform.ios) {
          this.props.imageCallback(pickerResult);
        } else {
          ImageStore.getBase64ForTag(
            pickerResult.uri,
            base64 => {
              pickerResult.base64 = base64;
              self.props.imageCallback(pickerResult);
            },
            error => console.log(error)
          );
        }


        this.setState({
          image: pickerResult
        });
      }
    } catch (e) {
      console.log({ uploadResponse });
      console.log({ uploadResult });
      console.log({ e });
      alert('Upload failed, sorry :(');
    } finally {
      this.setState({
        uploading: false
      });
    }
  };
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  exampleText: {
    fontSize: 20,
    marginBottom: 20,
    marginHorizontal: 15,
    textAlign: 'center',
  },
  maybeRenderUploading: {
    backgroundColor: 'rgba(0,0,0,0.4)'
  },
  maybeRenderContainer: {
    borderRadius: 3,
    elevation: 2,
    marginTop: 30,
    shadowColor: 'rgba(0,0,0,1)',
    shadowOpacity: 0.2,
    shadowOffset: {
      height: 4,
      width: 4,
    },
    shadowRadius: 5,
    width: 210,
  },
  maybeRenderImageContainer: {
    borderTopLeftRadius: 3,
    borderTopRightRadius: 3,
    overflow: 'hidden',
  },
  maybeRenderImage: {
    height: 297,
    width: 210,
  },
  maybeRenderImageText: {
    paddingHorizontal: 10,
    paddingVertical: 10,
  },
  primaryButton: {
    margin: 10,
    padding: 15,
    backgroundColor: "#4a76a8",
    alignSelf: 'auto',
    alignItems: 'center',
    justifyContent: 'center'
  }
});