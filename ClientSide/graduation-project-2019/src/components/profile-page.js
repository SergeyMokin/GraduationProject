import { Content, Container, List, ListItem, InputGroup, Input, Icon, Text, Button, Spinner } from 'native-base';
import React, {Component} from 'react';
import { AsyncStorage } from 'react-native';
import styles from '../styles/mainstyle.js';
import ApiRequests from '../api';

export default class ProfilePage extends Component {
  constructor(props){
      super(props);

      this.errorMessage = "";
      this.successMessage = "";

      this.state = {
        isLoading: false,
        email: "",
        oldPassword: "",
        newPassword: "",
        emailIconStyle: {
          color: 'blue'
        },
        passwordIconStyle: {
          color: 'blue'
        },
        messageStyle:{
          color: 'green',
          alignSelf: 'center',
          padding: 20
        }
    }

    this.api = new ApiRequests();
    this.api.setAuthorizationHeader(this.props.userInfo.bearerToken);
  }

  async changeEmail()
  {
    this.setState({
        isLoading: true
    });

    if(this.state.isLoading)
    {
        return;
    }

    let error = (error) => {
        this.successMessage = "";
        this.errorMessage = error.message;
        this.setState({
            isLoading: false,
            oldPassword: "",
            newPassword: "",
            emailIconStyle: {
                color: 'red'
            },
            passwordIconStyle: {
                color: 'blue'
            },
            messageStyle:{
              color: 'red',
              alignSelf: 'center',
              padding: 20
            }
        });
    };

    let success = (data) => {
        this.errorMessage = "";
        this.successMessage = "Success";
        
        this.setState({
            isLoading: false,
            email: "",
            oldPassword: "",
            newPassword: "",
            emailIconStyle: {
                color: 'blue'
            },
            passwordIconStyle: {
                color: 'blue'
            },
            messageStyle:{
              color: 'green',
              alignSelf: 'center',
              padding: 20
            }
        });

        let dataToChange = this.props.userInfo;

        dataToChange.user = data;

        AsyncStorage.setItem(dataToChange, JSON.stringify(data));

        this.props.changeUserInfo(data);
    };

    await this.api.changeEmail(this.state.email.toLowerCase())
        .then(success.bind(this))
        .catch(error.bind(this));
  }

  async changePassword()
  {

    this.setState({
        isLoading: true
    });

    if(this.state.isLoading)
    {
        return;
    }

    let error = (error) => {
        this.successMessage = "";
        this.errorMessage = error.status === 422 ? error.message + ". Password required 8 characters(1 uppercase, 1 lowercase, 1 digit, 1 special)" : error.message;
        this.setState({
            isLoading: false,
            email: "",
            emailIconStyle: {
                color: 'blue'
            },
            passwordIconStyle: {
                color: 'red'
            },
            messageStyle:{
              color: 'red',
              alignSelf: 'center',
              padding: 20
            }
        });
    };

    let success = (data) => {
        this.errorMessage = "";
        this.successMessage = "Success";
        
        this.setState({
            isLoading: false,
            email: "",
            oldPassword: "",
            newPassword: "",
            emailIconStyle: {
                color: 'blue'
            },
            passwordIconStyle: {
                color: 'blue'
            },
            messageStyle:{
              color: 'green',
              alignSelf: 'center',
              padding: 20
            }
        });
    };

    await this.api.changePassword(this.state.oldPassword, this.state.newPassword)
        .then(success.bind(this))
        .catch(error.bind(this));
  }

  render() {
    const content = this.state.isLoading ?
    <Content contentContainerStyle={styles.body}>
        <Spinner color="blue" />
    </Content>

    :
        <List>
            <ListItem itemHeader first style={{alignSelf: 'center', marginBottom: -25}}>
                <Text>PROFILE MANAGER</Text>
            </ListItem>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-person" style={this.state.emailIconStyle} />
                    <Input
                        onChangeText={(text) => this.setState({email: text})}
                        value={this.state.email}
                        placeholder={this.props.userInfo.user.email} />
                </InputGroup>
            </ListItem>                
            <Button style={styles.primaryButton} onPress={this.changeEmail.bind(this)}>
                <Text>change email</Text>
            </Button>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-unlock" style={this.state.passwordIconStyle} />
                    <Input
                        onChangeText={(text) => this.setState({oldPassword: text})}
                        value={this.state.oldPassword}
                        secureTextEntry={true}
                        placeholder={"Old password"} />
                </InputGroup>
            </ListItem>  
            <ListItem>
                <InputGroup>
                    <Icon name="ios-unlock" style={this.state.passwordIconStyle} />
                    <Input
                        onChangeText={(text) => this.setState({newPassword: text})}
                        value={this.state.newPassword}
                        secureTextEntry={true}
                        placeholder={"New password"} />
                </InputGroup>
            </ListItem>              
            <Button style={styles.primaryButton} onPress={this.changePassword.bind(this)}>
                <Text>change password</Text>
            </Button>
            <Button style={styles.primaryButton} onPress={this.props.logoutCallback}>
                <Text>logout</Text>
            </Button>
            <Text style={this.state.messageStyle}>{this.errorMessage !== "" ? this.errorMessage : this.successMessage !== "" ? this.successMessage : ""}</Text>
        </List>    
    ;
    return (
        <Content>
            {content}
        </Content>
    );
  }
}