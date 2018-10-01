import { Content, Text, List, ListItem, Spinner, Icon, Button, Item, Picker } from 'native-base';
import React, {Component} from 'react';
import ApiRequests from '../api';
import { Alert, Linking } from 'react-native';
import styles from '../styles/mainstyle.js';

export default class BlankListPage extends Component {
  constructor(props){
      super(props);

      this.state = {
        isLoading: false,
        isSend: false,
        fileToSend: null,
        selected2: {}
      }
      this.files = [];
      this.users = [];
      this.api = new ApiRequests();
      this.api.setAuthorizationHeader(this.props.userInfo.bearerToken);
  }

  async componentWillMount()
  {
    await this.getFiles();
    await this.getUsers();
  }

  async getUsers()
  {
    this.setState({isLoading: true});
    let error = (error) => {
      console.log(error);
      this.setState({isLoading: false});
    };

    let success = (data) => {
        this.users = data;
        this.onValueChange2(data[0]);
        this.setState({isLoading: false});
    };

    await this.api.getUsers()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  async getFiles()
  {
    this.setState({isLoading: true});
    let error = (error) => {
      console.log(error);
      this.setState({isLoading: false});
    };

    let success = (data) => {
        this.files = data;
        this.setState({isLoading: false});
    };

    await this.api.getFiles()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  alertMes(file)
  {
    if(file.isAccepted)
    {
      Alert.alert(
        'Choose what do you want to do with:',
        file.fileName,
        [
          {text: 'Close'},
          {text: 'Delete', onPress: () => { this.setState({isLoading: true}); this.api.removeFile(file.blankFileId).then(() => { this.getFiles(); }).catch((er) => { this.setState({isLoading: false}); Alert.alert(er); }) } },
          {text: 'Download', onPress: () => { this.setState({isLoading: true}); this.api.downloadFile(file.blankFileId).then((data) => { this.setState({isLoading: false}); Linking.openURL(data.uri); }).catch((er) => { this.setState({isLoading: false}); Alert.alert(er); }) } }
        ]
      );
    }
    else
    {
      Alert.alert(
        'Choose what do you want to do with:',
        file.fileName,
        [
          {text: 'Close'},
          {text: 'Delete', onPress: () => { this.setState({isLoading: true}); this.api.removeFile(file.blankFileId).then(() => { this.getFiles(); }).catch((er) => { this.setState({isLoading: false}); Alert.alert(er); }) } },
          {text: 'Accept', onPress: () => { this.setState({isLoading: true}); this.api.acceptFile(file.blankFileId).then((data) => { this.files = data; this.setState({isLoading: false}) }).catch((er) => { this.setState({isLoading: false}); Alert.alert(er); }) } }
        ]
      );
    }
  }

  async sendMessage()
  {
    this.setState({isLoading: true});
    console.log(this.state.selected2)
    let error = (error) => {
      console.log(error);
      this.setState({isLoading: false, isSend: false});
    };

    let success = (data) => {
        this.setState({isLoading: false, isSend: false});
    };

    await this.api.sendMessage({id: this.state.selected2.id, fileIds: [ this.state.fileToSend.blankFileId ]})
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  onValueChange2(value)
  {
    this.setState({
        selected2: value
      });
  }

  render() {
    if(this.state.isLoading)
    {
      return  <Content contentContainerStyle={styles.body}>
                <Spinner color="blue" />
              </Content>
    }
    else if(this.state.isSend)
    {
      return  <Content>
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
                      {this.users.map(item => <Picker.Item label={item.email} value={item.email} key={item.id} />)}
                      </Picker>
                </Item>
                <Button style={styles.primaryButton} onPress={() => this.setState({isSend: false})}><Text>Back</Text></Button>
                <Button style={styles.primaryButton} onPress={this.sendMessage.bind(this)}><Text>Send</Text></Button>
              </Content>
    }
    return (
            <Content>  
              <List>
                <ListItem itemHeader first>
                  <Text>YOUR GENERATED BLANKS</Text>
                </ListItem>
                {this.files.map((file) => 
                  <ListItem key={file.blankFileId} 
                    onLongPress={() => {this.setState({isSend: true, fileToSend: file})}} onPress={() => this.alertMes(file)}>
                  <Icon name="ios-document" style={{margin: 5, color: file.isAccepted ? 'blue' : 'yellow'}} />
                  <Text style={{padding: 5}}>{file.blankFileId}: {file.fileName}</Text>
                </ListItem>)}
              </List>      
            </Content>
    );
  }
}