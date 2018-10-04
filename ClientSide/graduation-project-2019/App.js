import React from 'react';
import { StatusBar, AsyncStorage, Image } from 'react-native';
import MainPage from './src/components/main-page';
import { Container, Content, Spinner, Header } from 'native-base';
import styles from './src/styles/mainstyle.js';
import { Font } from 'expo';
import LoginPage from './src/components/login-page';
import ApiRequsts from './src/api'

export default class App extends React.Component {

  constructor(props) {
    super(props);
    this.state = { isLoading: true, isLogined: false };
    this.userInfo = {};
    this.api = new ApiRequsts();
    this.userInfoContainer = this.api.asyncStorageUser;
  }

  changeUserInfo(data)
  {
    this.userInfo = data;
  }

  async componentWillMount() {
    await Font.loadAsync({
      Roboto: require("native-base/Fonts/Roboto.ttf"),
      Roboto_medium: require("native-base/Fonts/Roboto_medium.ttf")
    });

    await this.tryGetUserInfo();
  }

  async tryGetUserInfo()
  {
    let successUpdate = (data) => {
      this.userInfo = data;
      AsyncStorage.setItem(this.userInfoContainer, JSON.stringify(data));
      this.setState({isLogined: true, isLoading: false});
    };

    let success = (data) => {
      if(data === null) 
      {
        this.setState({isLogined: false, isLoading: false});
        return;
      }

      this.api.setAuthorizationHeader(JSON.parse(data).bearerToken);
      this.api.updateToken()
        .then(successUpdate.bind(this))
        .catch(error);
    };

    let error = (error) => {
      console.log(error);
      this.setState({ isLoading: false });
    }

    await AsyncStorage.getItem(this.userInfoContainer)
      .then(success.bind(this))
      .catch(error);
  }

  async authCallback()
  {
    let success = (data) => {
      if(data !== null)
      {
        this.userInfo = JSON.parse(data);
        this.setState({isLogined: true});
      }
      else
      {
        this.setState({isLogined: false});
      }
    };
    let error = (error) => {
      console.log(error);
      this.setState({isLogined: false});
    }
    await AsyncStorage.getItem(this.userInfoContainer)
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  render() {
    const content = this.state.isLoading ?
      <Content contentContainerStyle={styles.body}>
        <Spinner color="blue" />
      </Content>

      : this.state.isLogined ?
      <MainPage userInfo = {this.userInfo} logout={this.authCallback.bind(this)} changeUserInfo = {this.changeUserInfo.bind(this)}/>

      :
      <LoginPage loginSuccessful={this.authCallback.bind(this)}/>
    ;

    return (
      <Container>
        <StatusBar hidden={true} />
        
        <Header style={{backgroundColor:'blue'}}>
          <Image source={require('./src/images/gp-logo-white.png')} style={{width:50, height: 50}}/>
        </Header>
        {content}
      </Container>
    );
  }
}

