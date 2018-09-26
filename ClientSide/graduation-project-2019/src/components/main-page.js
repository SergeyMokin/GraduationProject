import React from 'react';
import { AsyncStorage } from 'react-native';
import { Container, Button, Footer, FooterTab, Icon, Content } from 'native-base';
import ApiRequsts from '../api';
import ProfilePage from './profile-page';
import BlankCreatorPage from './blank-creator-page';
import BlankListPage from './blank-list-page';

export default class MainPage extends React.Component {
    static routes = {
        main: "home",
        profile: "person",
        blankCreator: "document"
    };

    constructor(props) {
        super(props);

        this.state = {
            currentPage: MainPage.routes.main
        };

        this.api = new ApiRequsts();
    }
  
    logout(){
        AsyncStorage.removeItem(this.api.asyncStorageUser);
        this.props.logout();
    }

    changePage(name){
        this.setState({
            currentPage: name
        });
    }

    renderButton(name)
    {
        if(this.state.currentPage === name)
        {
            return  <Button active style={{ backgroundColor: "blue" }} onPress={() => this.changePage(name)}>
                        <Icon active name={name} />
                    </Button>
        }
        else
        {
            return  <Button style={{ backgroundColor: "blue" }} onPress={() => this.changePage(name)}>
                        <Icon name={name} />
                    </Button>
        }
    }

    render() {
        const content = this.state.currentPage === MainPage.routes.main ?
            <BlankListPage userInfo={this.props.userInfo} />

            : this.state.currentPage === MainPage.routes.profile ?
            <ProfilePage userInfo={this.props.userInfo} logoutCallback = {this.logout.bind(this)} changeUserInfo = {this.props.changeUserInfo}/>

            :
            <BlankCreatorPage userInfo={this.props.userInfo} />
            ;

        const buttonContent = 
        <FooterTab style={{ backgroundColor: "blue" }}>
            {this.renderButton(MainPage.routes.main)}
            {this.renderButton(MainPage.routes.profile)}
            {this.renderButton(MainPage.routes.blankCreator)}
        </FooterTab>
        ;

        return (
            <Container>
                {content}
                
                <Footer style={{ backgroundColor: "blue" }}>
                    {buttonContent}
                </Footer>
            </Container>
        )
    }
}
