import { Content, List, ListItem, InputGroup, Input, Icon, Text, Button, Spinner, Toast } from 'native-base';
import React, { Component } from 'react';
import { AsyncStorage } from 'react-native';
import styles from '../styles/mainstyle.js';
import ApiRequests from '../api';

export default class LoginPage extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isLoading: false,
            email: "",
            password: "",
            iconStyle: {
                color: '#4a76a8'
            }
        }

        this.errorMessage = "";
        this.api = new ApiRequests();
    }

    showInfoMessage(message) {
        Toast.show({
            text: message,
            buttonText: 'Okay',
            duration: 5000
        })
    }

    async login() {
        await this.action();
    }

    async register() {
        await this.action(true);
    }

    async action(isRegister = false) {
        this.setState({
            isLoading: true
        });

        if (this.state.isLoading) {
            return;
        }

        let error = (error) => {
            this.errorMessage = error.status === 422
                ? error.message + ". Password required 8 characters(1 uppercase, 1 lowercase, 1 digit, 1 special)"
                : error.status === 500 ? error.message + ". User exists"
                    : error.message;

            this.showInfoMessage(this.errorMessage);
            this.setState({
                isLoading: false,
                iconStyle: {
                    color: '#ff4d4d'
                }
            });
        };

        let success = (data) => {
            this.errorMessage = "";

            this.setState({
                isLoading: false,
                email: "",
                password: "",
                iconStyle: {
                    color: '#4a76a8'
                }
            });

            AsyncStorage.setItem(this.api.asyncStorageUser, JSON.stringify(data));

            this.props.loginSuccessful();
        };

        if (isRegister) {
            await this.api.register(this.state.email.toLowerCase(), this.state.password)
                .then(success.bind(this))
                .catch(error.bind(this));
        }
        else {
            await this.api.login(this.state.email.toLowerCase(), this.state.password)
                .then(success.bind(this))
                .catch(error.bind(this));
        }
    }

    render() {
        const content = this.state.isLoading
            ?

            <Content contentContainerStyle={styles.body}>
                <Spinner color="#4a76a8" />
            </Content>
            :

            <Content>
                <List>
                    <ListItem>
                        <InputGroup>
                            <Icon name="ios-person" style={this.state.iconStyle} />
                            <Input
                                onChangeText={(text) => this.setState({ email: text })}
                                value={this.state.email}
                                placeholder={"Email Address"} />
                        </InputGroup>
                    </ListItem>
                    <ListItem>
                        <InputGroup>
                            <Icon name="ios-unlock" style={this.state.iconStyle} />
                            <Input
                                onChangeText={(text) => this.setState({ password: text })}
                                value={this.state.password}
                                secureTextEntry={true}
                                placeholder={"Password"} />
                        </InputGroup>
                    </ListItem>
                </List>
                <Button style={styles.primaryButton} onPress={this.login.bind(this)}>
                    <Text>sign in</Text>
                </Button>
                <Button style={styles.primaryButton} onPress={this.register.bind(this)}>
                    <Text>sign up</Text>
                </Button>
            </Content>
            ;

        return (content);
    }
}