import React from 'react';
import { StatusBar, AsyncStorage } from 'react-native';
import Test from './src/components/test';
import { Container, Content, Spinner } from 'native-base';
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
      if(data === null) return;

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

  async loginSuccessful()
  {
    let success = (data) => {
      this.userInfo = JSON.parse(data);
      this.setState({isLogined: true});
    };
    let error = (error) => {
      console.log(error);
    }
    await AsyncStorage.getItem(this.userInfoContainer)
      .then(success.bind(this))
      .catch(error);
  }

  render() {
    if(this.state.isLoading)
    {
      return (
      <Container>
        <StatusBar hidden={true} />
        <Content contentContainerStyle={styles.body}>
            <Spinner color="blue" />
        </Content>
      </Container>
      );
    }
    if(this.state.isLogined)
    {
      return (
      <Container>
        <StatusBar hidden={true} />
        <Test userInfo = {this.userInfo}/>
      </Container>
      )
    }
    return (
      <Container>
        <StatusBar hidden={true} />
        <LoginPage loginSuccessful={this.loginSuccessful.bind(this)}/>
      </Container>
    );
  }
}

