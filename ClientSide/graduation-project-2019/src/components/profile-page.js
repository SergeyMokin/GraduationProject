import { Content, List, ListItem, InputGroup, Input, Icon, Text, Button, Spinner, Toast } from 'native-base';
import React, { Component } from 'react';
import { AsyncStorage, BackHandler } from 'react-native';
import styles from '../styles/mainstyle.js';
import ApiRequests from '../api';

const VIEWS = {
    main: 'main',
    password: 'password',
    email: 'email'
};

const LIST_BUTTON_STYLE = {
    alignSelf: 'stretch',
    backgroundColor: 'white',
    marginBottom: 0.5
}

export default class ProfilePage extends Component {
    constructor(props) {
        super(props);

        this.backHandler = null;

        this.state = {
            isLoading: false,
            email: "",
            oldPassword: "",
            newPassword: "",
            emailIconStyle: {
                color: '#4a76a8'
            },
            passwordIconStyle: {
                color: '#4a76a8'
            },
            currentPage: VIEWS.main
        }

        this.api = new ApiRequests();
        this.api.setAuthorizationHeader(this.props.userInfo.bearerToken);
    }

    async changeEmail() {
        this.setState({
            isLoading: true
        });
        this.props.footerDisableCallback(true);

        if (this.state.isLoading) {
            return;
        }

        let error = (error) => {
            this.showInfoMessage(error.message);
            this.setState({
                isLoading: false,
                oldPassword: "",
                newPassword: "",
                emailIconStyle: {
                    color: '#ff4d4d'
                },
                passwordIconStyle: {
                    color: '#4a76a8'
                }
            });
            this.props.footerDisableCallback(false);
        };

        let success = (data) => {
            this.showInfoMessage("Success");

            this.setState({
                isLoading: false,
                email: "",
                oldPassword: "",
                newPassword: "",
                emailIconStyle: {
                    color: '#4a76a8'
                },
                passwordIconStyle: {
                    color: '#4a76a8'
                }
            });
            this.props.footerDisableCallback(false);

            let dataToChange = this.props.userInfo;

            dataToChange.user = data;

            AsyncStorage.setItem(dataToChange, JSON.stringify(data));

            this.props.changeUserInfo(data);
        };

        await this.api.changeEmail(this.state.email.toLowerCase())
            .then(success.bind(this))
            .catch(error.bind(this));
    }

    showInfoMessage(message) {
        Toast.show({
            text: message,
            buttonText: 'Okay',
            duration: 5000
        })
    }

    back() {
        this.setState({
            currentPage: VIEWS.main,
            emailIconStyle: {
                color: '#4a76a8'
            },
            passwordIconStyle: {
                color: '#4a76a8'
            }
        });
        return true;
    }

    async changePassword() {

        this.setState({
            isLoading: true
        });
        this.props.footerDisableCallback(true);

        if (this.state.isLoading) {
            return;
        }

        let error = (error) => {
            this.showInfoMessage(error.status === 422 ? error.message + ". Password required 8 characters(1 uppercase, 1 lowercase, 1 digit, 1 special)" : error.message);
            this.setState({
                isLoading: false,
                email: "",
                emailIconStyle: {
                    color: '#4a76a8'
                },
                passwordIconStyle: {
                    color: '#ff4d4d'
                }
            });
            this.props.footerDisableCallback(false);
        };

        let success = (data) => {
            this.showInfoMessage("Success");

            this.setState({
                isLoading: false,
                email: "",
                oldPassword: "",
                newPassword: "",
                emailIconStyle: {
                    color: '#4a76a8'
                },
                passwordIconStyle: {
                    color: '#4a76a8'
                }
            });
            this.props.footerDisableCallback(false);
        };

        await this.api.changePassword(this.state.oldPassword, this.state.newPassword)
            .then(success.bind(this))
            .catch(error.bind(this));
    }

    render() {
        if (this.backHandler !== null) {
            this.backHandler.remove();
            this.backHandler = null;
        }
        if (this.state.currentPage === VIEWS.password
            || this.state.currentPage === VIEWS.email) {
            this.backHandler = BackHandler.addEventListener('hardwareBackPress', this.back.bind(this));
        }
        const content = this.state.isLoading ?
            <Content contentContainerStyle={styles.body}>
                <Spinner color="#4a76a8" />
            </Content>

            : this.state.currentPage === VIEWS.password
                ? <Content>
                    <List>
                        <ListItem itemHeader first style={{ alignSelf: 'center', marginBottom: -25 }}>
                            <Text>PROFILE MANAGER</Text>
                        </ListItem>
                        <ListItem>
                            <InputGroup>
                                <Icon name="ios-unlock" style={this.state.passwordIconStyle} />
                                <Input
                                    onChangeText={(text) => this.setState({ oldPassword: text })}
                                    value={this.state.oldPassword}
                                    secureTextEntry={true}
                                    placeholder={"Old password"} />
                            </InputGroup>
                        </ListItem>
                        <ListItem>
                            <InputGroup>
                                <Icon name="ios-unlock" style={this.state.passwordIconStyle} />
                                <Input
                                    onChangeText={(text) => this.setState({ newPassword: text })}
                                    value={this.state.newPassword}
                                    secureTextEntry={true}
                                    placeholder={"New password"} />
                            </InputGroup>
                        </ListItem>
                        <Button style={styles.primaryButton} onPress={this.changePassword.bind(this)}>
                            <Text>change</Text>
                        </Button>
                    </List>
                </Content>

                : this.state.currentPage === VIEWS.email
                    ? <Content>
                        <List>
                            <ListItem itemHeader first style={{ alignSelf: 'center', marginBottom: -25 }}>
                                <Text>PROFILE MANAGER</Text>
                            </ListItem>
                            <ListItem>
                                <InputGroup>
                                    <Icon name="ios-person" style={this.state.emailIconStyle} />
                                    <Input
                                        onChangeText={(text) => this.setState({ email: text })}
                                        value={this.state.email}
                                        placeholder={this.props.userInfo.user.email} />
                                </InputGroup>
                            </ListItem>
                            <Button style={styles.primaryButton} onPress={this.changeEmail.bind(this)}>
                                <Text>change</Text>
                            </Button>
                        </List>
                    </Content>

                    : <Content>
                        <List>
                            <ListItem itemHeader first style={{ alignSelf: 'center', marginBottom: -25 }}>
                                <Text>PROFILE MANAGER</Text>
                            </ListItem>
                            <Button style={LIST_BUTTON_STYLE} onPress={() => this.setState({ currentPage: VIEWS.email })}>
                                <Text style={{ color: 'black' }}>change email</Text>
                            </Button>
                            <Button style={LIST_BUTTON_STYLE} onPress={() => this.setState({ currentPage: VIEWS.password })}>
                                <Text style={{ color: 'black' }}>change password</Text>
                            </Button>
                            <Button style={LIST_BUTTON_STYLE} onPress={this.props.logoutCallback}>
                                <Text style={{ color: 'black' }}>logout</Text>
                            </Button>
                        </List>
                    </Content>
            ;
        return (content);
    }
}