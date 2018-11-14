import { Content, Text, List, ListItem, Spinner, Icon, Button, Item, Picker, Thumbnail, ActionSheet } from 'native-base';
import React, { Component } from 'react';
import ApiRequests from '../api';
import { Alert, Linking, BackHandler } from 'react-native';
import styles from '../styles/mainstyle.js';

const ACCEPTED_BUTTONS = [
  { text: "Share", icon: "ios-share-alt", iconColor: "#ff9966" },
  { text: "Download", icon: "download", iconColor: "#4a76a8" },
  { text: "Delete", icon: "trash", iconColor: "#ff4d4d" }
];
let NOT_ACCEPTED_BUTTONS = [
  { text: "Accept", icon: "ios-checkmark-circle", iconColor: "#ff9966" },
  { text: "Delete", icon: "trash", iconColor: "#ff4d4d" }
];

export default class BlankListPage extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      isSend: false,
      fileToSend: null,
      selected2: {}
    }
    this.files = [];
    this.users = [];
    this.backHandler = null;
    this.api = new ApiRequests();
    this.api.setAuthorizationHeader(this.props.userInfo.bearerToken);
  }

  async componentWillMount() {
    await this.getFiles();
    await this.getUsers();
  }

  async getUsers() {
    this.setState({ isLoading: true });
    this.props.footerDisableCallback(true);
    let error = (error) => {
      console.log(error);
      this.setState({ isLoading: false });
    };

    let success = (data) => {
      this.users = data;
      this.onValueChange2(data[0]);
      this.setState({ isLoading: false });
      this.props.footerDisableCallback(false);
    };

    await this.api.getUsers()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  async getFiles() {
    this.setState({ isLoading: true });
    this.props.footerDisableCallback(true);
    let error = (error) => {
      console.log(error);
      this.setState({ isLoading: false });
      this.props.footerDisableCallback(false);
    };

    let success = (data) => {
      this.files = data;
      this.setState({ isLoading: false });
      this.props.footerDisableCallback(false);
    };

    await this.api.getFiles()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  alertMes(file) {
    if (file.isAccepted) {
      ActionSheet.show(
        {
          options: ACCEPTED_BUTTONS,
          destructiveButtonIndex: 2,
          title: 'Choose what do you want to do with: ' + file.blankFileId
        },
        buttonIndex => {
          switch (buttonIndex) {
            case 0:
              this.sendMessageModalOpen(file);
              break;
            case 1:
              this.setState({ isLoading: true });
              this.props.footerDisableCallback(true);
              this.api.downloadFile(file.blankFileId)
                .then((data) => { this.setState({ isLoading: false }); this.props.footerDisableCallback(false); Linking.openURL(data); })
                .catch((er) => { this.setState({ isLoading: false }); this.props.footerDisableCallback(false); Alert.alert(er); });
              break;
            case 2:
              Alert.alert(
                'Delete file: ' + file.blankFileId,
                'Are you sure?',
                [
                  { text: 'Cancel', style: 'cancel' },
                  {
                    text: 'OK', onPress: () => {
                      this.setState({ isLoading: true });
                      this.props.footerDisableCallback(true);
                      this.api.removeFile(file.blankFileId)
                        .then(() => { this.getFiles(); })
                        .catch((er) => { this.setState({ isLoading: false }); this.props.footerDisableCallback(false); Alert.alert(er); });
                    }
                  },
                ],
                { cancelable: false }
              )
              break;
            default:
              break;
          }
        }
      );
    }
    else {
      ActionSheet.show(
        {
          options: NOT_ACCEPTED_BUTTONS,
          destructiveButtonIndex: 1,
          title: 'Choose what do you want to do with file: ' + file.blankFileId
        },
        buttonIndex => {
          switch (buttonIndex) {
            case 0:
              this.setState({ isLoading: true });
              this.props.footerDisableCallback(true);
              this.api.acceptFile(file.blankFileId)
                .then((data) => { this.files = data; this.setState({ isLoading: false }); this.props.footerDisableCallback(false); })
                .catch((er) => { this.setState({ isLoading: false }); this.props.footerDisableCallback(false); Alert.alert(er); });
              break;
            case 1:
              this.setState({ isLoading: true });
              this.api.removeFile(file.blankFileId)
                .then(() => { this.getFiles(); })
                .catch((er) => { this.setState({ isLoading: false }); this.props.footerDisableCallback(false); Alert.alert(er); });
              break;
            default:
              break;
          }
        }
      );
    }
  }

  async sendMessage() {
    this.setState({ isLoading: true });
    this.props.footerDisableCallback(true);
    let error = (error) => {
      console.log(error);
      this.setState({ isLoading: false, isSend: false });
      this.props.footerDisableCallback(false);
    };

    let success = (data) => {
      this.setState({ isLoading: false, isSend: false });
      this.props.footerDisableCallback(false);
    };

    await this.api.sendMessage({ id: this.state.selected2.id, fileIds: [this.state.fileToSend.blankFileId] })
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  onValueChange2(value) {
    this.setState({
      selected2: value
    });
  }

  sendMessageModalOpen(file) {
    if (file.isAccepted) {
      this.setState({ isSend: true, fileToSend: file });
    }
  }

  sendMessageModalClose() {
    this.setState({ isSend: false });
    return true;
  }

  render() {
    if (this.backHandler !== null) {
      this.backHandler.remove();
      this.backHandler = null;
    }
    if (this.state.isLoading) {
      return <Content contentContainerStyle={styles.body}>
        <Spinner color="#4a76a8" />
      </Content>
    }
    else if (this.state.isSend) {
      this.backHandler = BackHandler.addEventListener('hardwareBackPress', this.sendMessageModalClose.bind(this));
      return <Content>
        <Item picker>
          <Picker
            mode="dropdown"
            iosIcon={<Icon name="ios-arrow-down-outline" />}
            style={{ width: undefined }}
            placeholder="Select blank type"
            placeholderStyle={{ color: "#bfc6ea" }}
            placeholderIconColor="#007aff"
            selectedValue={this.state.selected2}
            onValueChange={this.onValueChange2.bind(this)}
          >
            {this.users.map(item => <Picker.Item label={item.email} value={item} key={item.id} />)}
          </Picker>
        </Item>
        <Button style={styles.primaryButton} onPress={this.sendMessage.bind(this)}><Text>Share</Text></Button>
      </Content>
    }
    else if (this.files.length === 0) {
      return <Content>
        <Text style={{ alignSelf: 'center', padding: 5, color: 'black' }}>Your list of blanks is empty</Text>
      </Content>
    }
    return (
      <Content>
        <List>
          <ListItem itemHeader first style={{ alignSelf: 'center', marginBottom: -25 }}>
            <Text>LIST OF YOUR BLANKS</Text>
          </ListItem>
          {this.files.map((file) =>
            <ListItem key={file.blankFileId}
              onPress={() => this.alertMes(file)}>
              <Thumbnail tintColor={file.isAccepted ? '#4a76a8' : '#ff9966'} square small source={require('../images/excel_file_icon.png')} />
              <Text style={{ padding: 5, color: file.isAccepted ? 'black' : '#ff9966' }}>{file.blankFileId}</Text>
              <Text style={{ fontSize: 10, color: file.isAccepted ? 'gray' : '#ff9966' }} note>{file.isAccepted ? file.fileName : 'NOT ACCEPTED FILE'}</Text>
            </ListItem>)}
        </List>
      </Content>
    );
  }
}